using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BLockingArrayQueue;
using NUnit.Framework;

namespace BlockingArrayQueue
{
    abstract class BlockingArrayAbstractTest
    {
        private IBlockingArrayQueue<int> _queue;

        private int _acc;
        private const int Limit = 10000;
        private const int Sum = Limit * (Limit - 1) / 2;

        private const int Repeat = 10;
        private bool _sequential;
        private volatile bool _settersFinished;

        private void SetRunner()
        {
            int next;
            while ((next = Interlocked.Increment(ref _acc)) < Limit)
            {
                _queue.Enqueue(next);
            }
        }

        private int GetRunner()
        {
            int sum = 0;
            int prev = 0;
            while (true)
            {
                int currentValue;
                if (_queue.TryDequeue(out currentValue))
                {
                    if (_sequential)
                    {
                        Assert.Less(prev, currentValue);
                    }
                    sum += currentValue;
                    prev = currentValue;
                }
                else if (_settersFinished)
                {
                    break;
                }
            }

            return sum;
        }

        private void RunSetters(int count)
        {
            Task[] tasks = new Task[count];
            for (int i = 0; i < count; i++) tasks[i] = Task.Factory.StartNew(SetRunner, TaskCreationOptions.LongRunning);
            new TaskFactory().ContinueWhenAll(tasks, _ => _settersFinished = true);
        }

        private int RunGettersAndWait(int count)
        {
            var tasks = new Task<int>[count];
            for (int i = 0; i < count; i++) tasks[i] = Task.Factory.StartNew(GetRunner, TaskCreationOptions.LongRunning);

            return tasks.Aggregate(0, (sum, task) => sum + task.Result);
        }


        protected abstract IBlockingArrayQueue<int> CreateQueue();

        [SetUp]
        public void SetUp()
        {
            _acc = 0;
            _sequential = true;
            _settersFinished = false;
            _queue = CreateQueue();
        }

        [Test]
        public void TestOneThread()
        {
            _queue.Enqueue(1);
            _queue.Enqueue(2);

            int result;
            Assert.True(_queue.TryDequeue(out result) && result == 1);
            Assert.True(_queue.TryDequeue(out result) && result == 2);
            Assert.False(_queue.TryDequeue(out result));
        }


        [Test, Repeat(Repeat)]
        public void TestOneSetterAndOneGetter()
        {
            RunSetters(1);
            Assert.AreEqual(Sum, RunGettersAndWait(1));
        }

        [Test, Repeat(Repeat)]
        public void TestOneSettersAndSeveralGetters()
        {
            RunSetters(1);
            Assert.AreEqual(Sum, RunGettersAndWait(2));
        }

        [Test, Repeat(Repeat)]
        public void TestSeveralSettersAndSeveralGetters()
        {
            _sequential = false;
            RunSetters(3);
            Assert.AreEqual(Sum, RunGettersAndWait(3));
        }

        [Test, Repeat(Repeat)]
        public void TestManySettersAndManyGetters()
        {
            _sequential = false;
            RunSetters(10);
            Assert.AreEqual(Sum, RunGettersAndWait(10));
        }
    }

    internal class LockedBlockingArrayQueueTests : BlockingArrayAbstractTest
    {
        protected override IBlockingArrayQueue<int> CreateQueue()
        {
            return new LockedBlockingArrayQueue<int>();
        }
    }

    internal class LockFreeBlockingArrayQueueTests : BlockingArrayAbstractTest
    {
        protected override IBlockingArrayQueue<int> CreateQueue()
        {
            return new LockFreeBlockingArrayQueue<int>();
        }
    }
}
