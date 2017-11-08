namespace BLockingArrayQueue
{
    internal abstract class AbstractBlockingArrayQueue<T> : IBlockingArrayQueue<T>
    {
        public abstract bool TryEnqueue(T item);

        public void Enqueue(T res)
        {
            while (!TryEnqueue(res))
            {
            }
        }

        public abstract bool TryDequeue(out T item);

        public T Dequeue()
        {
            T res;
            while (!TryDequeue(out res))
            {
            }
            return res;
        }

        public abstract void Clear();
    }
}
