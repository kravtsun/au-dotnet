using System.Threading;

namespace Dinner
{
    internal interface IFork
    {
        int Id { get; }

        bool Get();

        void Put();
    }

    internal class Fork : IFork
    {
        public int Id { get; }

        private readonly Mutex _mutex;

        public Fork(int id)
        {
            Id = id;
            _mutex = new Mutex(false);
        }

        public bool Get()
        {
            _mutex.WaitOne();
            return true;
        }

        public void Put()
        {
            _mutex.ReleaseMutex();
        }
    }
}
