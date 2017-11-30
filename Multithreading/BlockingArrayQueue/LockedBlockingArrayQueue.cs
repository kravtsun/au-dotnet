using System.Collections.Generic;

namespace BLockingArrayQueue
{
    class LockedBlockingArrayQueue<T> : AbstractBlockingArrayQueue<T>
    {
        private readonly LinkedList<T> _queue = new LinkedList<T>();
        
        public override bool TryEnqueue(T item)
        {
            lock (_queue)
            {
                _queue.AddLast(item);
                return true;
            }
        }

        public override bool TryDequeue(out T item)
        {
            lock (_queue)
            {
                if (_queue.Count == 0)
                {
                    item = default(T);
                    return false;
                }
                item = _queue.First.Value;
                _queue.RemoveFirst();
                return true;
            }
        }

        public override void Clear()
        {
            lock (_queue)
            {
                _queue.Clear();
            }
        }
    }
}
