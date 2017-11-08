namespace BLockingArrayQueue
{
    public interface IBlockingArrayQueue<T>
    {
        bool TryEnqueue(T item);

        void Enqueue(T item);

        bool TryDequeue(out T item);

        T Dequeue();

        void Clear();
    }
}
