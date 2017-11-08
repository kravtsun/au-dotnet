using System;
using System.Threading;

namespace BLockingArrayQueue
{
    class LockFreeBlockingArrayQueue<T> : AbstractBlockingArrayQueue<T>
    {
        private HeadTailHolder _headTail = new HeadTailHolder();

        public override bool TryEnqueue(T v)
        {
            var newNode = new Node(v, null, true);
            var currentTail = _headTail.Tail;
            var next = currentTail.NextNode;
            if (next != null)
            {
                return Interlocked.CompareExchange(ref _headTail.Tail, next, currentTail) == currentTail;
            }
            return null == Interlocked.CompareExchange(ref _headTail.Tail.NextNode, newNode, null);
        }

        public override bool TryDequeue(out T res)
        {
            while (true)
            {
                // invariant - head is always points to last "logically removed" entry 
                var h = _headTail.Head;
                if (h.IsAlive) throw new InvalidOperationException("head.isAlive");

                var next = h.NextNode;
                if (next == null)
                {
                    res = default(T);
                    return false;
                }

                if (next.IsAlive && Interlocked.CompareExchange(ref next.AliveState, 0, 1) == 1)
                {
                    res = next.Value;
                    return true;
                }

                // physical removal.
                Interlocked.CompareExchange(ref _headTail.Head, next, h);
            }
        }

        public override void Clear()
        {
            var newHeadTail = new HeadTailHolder();
            Interlocked.Exchange(ref _headTail, newHeadTail);
        }

        private class Node
        {
            internal readonly T Value;
            internal Node NextNode;
            internal int AliveState;
            internal bool IsAlive => AliveState != 0;

            public Node(T value, Node nextNode, bool isAlive)
            {
                Value = value;
                NextNode = nextNode;
                AliveState = isAlive ? 1 : 0;
            }
        }

        private class HeadTailHolder
        {
            internal Node Head;
            internal Node Tail;

            public HeadTailHolder()
            {
                var markedNode = new Node(default(T), null, false);
                Head = markedNode;
                Tail = markedNode;
            }
        }
    }
}
