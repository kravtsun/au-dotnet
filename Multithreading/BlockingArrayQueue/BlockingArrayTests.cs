using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BLockingArrayQueue;
using NUnit.Framework;

namespace BlockingArrayQueue
{
    abstract class BlockingArrayAbstractTest
    {
        private IBlockingArrayQueue<int> queue;

        private int _acc;
        private const int Limit = 10000;
        private const long Sum = Limit * (Limit - 1) / 2;

        private const int Repeat = 10;
        private bool sequential;
        private volatile bool settersFinished;

        private void SetRunner()
        {
            int next;
            while ((next = Interlocked.Increment(ref _acc)) < Limit)
            {
                queue.Enqueue(next);
            }
        }

        private long GetRunner()
        {
            long sum = 0;
            long prev = 0;
            while (true)
            {
                int currentValue;
                if (queue.TryDequeue(out currentValue))
                {
                    if (sequential)
                    {
                        Assert.Less(prev, currentValue);
                    }
                    sum += currentValue;
                    prev = currentValue;
                }
                else if (settersFinished)
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
            new TaskFactory().ContinueWhenAll(tasks, _ => settersFinished = true);
        }

        private long RunGettersAndWait(int count)
        {
            var tasks = new Task<long>[count];
            for (int i = 0; i < count; i++) tasks[i] = Task.Factory.StartNew(GetRunner, TaskCreationOptions.LongRunning);

            return tasks.Aggregate(0L, (sum, task) => sum + task.Result);
        }


        protected abstract IBlockingArrayQueue<int> CreateQueue();

        [SetUp]
        public void SetUp()
        {
            _acc = 0;
            sequential = true;
            settersFinished = false;
            queue = CreateQueue();
        }

        [Test]
        public void TestOneThread()
        {
            queue.Enqueue(1);
            queue.Enqueue(2);

            int result;
            Assert.True(queue.TryDequeue(out result) && result == 1);
            Assert.True(queue.TryDequeue(out result) && result == 2);
            Assert.False(queue.TryDequeue(out result));
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
            sequential = false;
            RunSetters(3);
            Assert.AreEqual(Sum, RunGettersAndWait(3));
        }

        [Test, Repeat(Repeat)]
        public void TestManySettersAndManyGetters()
        {
            sequential = false;
            RunSetters(10);
            Assert.AreEqual(Sum, RunGettersAndWait(10));
        }
    }

    class LockedBlockingArrayQueueTests : BlockingArrayAbstractTest
    {
        protected override IBlockingArrayQueue<int> CreateQueue()
        {
            return new LockedBlockingArrayQueue<int>();
        }
    }

    class LockFreeBlockingArrayQueueTests : BlockingArrayAbstractTest
    {
        protected override IBlockingArrayQueue<int> CreateQueue()
        {
            return new LockFreeBlockingArrayQueue<int>();;
        }
    }
}
