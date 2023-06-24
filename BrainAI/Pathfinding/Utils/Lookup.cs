using System.Collections.Generic;
using System;
using System.Collections;
using System.Linq;

namespace BrainAI.Pathfinding
{
    public class Lookup<TKey, TValue> : ILookup<TKey, TValue>
    {
        internal Dictionary<TKey, LinkedListNode<(TKey, TValue)>> bucketReference = new Dictionary<TKey, LinkedListNode<(TKey, TValue)>>();
        internal LinkedList<(TKey, TValue)> valuesList = new LinkedList<(TKey, TValue)>();

        internal int count;
        internal int version;

        public Lookup()
        {
        }

        public int Count
        {
            get { return count; }
        }

        public void Add(TKey key, TValue value)
        {
            if (bucketReference.ContainsKey(key))
            {
                valuesList.AddAfter(bucketReference[key], (key, value));
            }
            else
            {
                var node = valuesList.AddLast((key, value));
                bucketReference[key] = node;
            }
            version++;
            count++;
        }

        public void Remove(TKey key, TValue value)
        {
            if (!bucketReference.ContainsKey(key))
            {
                return;
            }

            var start = bucketReference[key];
            while (
                EqualityComparer<TKey>.Default.Equals(start.Value.Item1, key) &&
                !EqualityComparer<TValue>.Default.Equals(start.Value.Item2, value))
            {
                start = start.next;
            }

            if (!EqualityComparer<TKey>.Default.Equals(start.Value.Item1, key))
            {
                return;
            }

            if (start == bucketReference[key])
            {
                if (start.Next == null || !EqualityComparer<TKey>.Default.Equals(start.Next.Value.Item1, key))
                {
                    bucketReference.Remove(key);
                }
                else
                {
                    bucketReference[key] = bucketReference[key].Next;
                }
            }

            valuesList.Remove(start);
            version++;
            count--;
        }

        public void Clear()
        {
            valuesList.Clear();
            bucketReference.Clear();
            count = 0;
            version++;
        }

        private Enumerable Find(TKey key)
        {
            if (!bucketReference.ContainsKey(key))
            {
                return new Enumerable(null);
            }

            return new Enumerable(bucketReference[key]);
        }

        public bool Contains(TKey key)
        {
            return bucketReference.ContainsKey(key);
        }

        public IEnumerable<TValue> this[TKey key] => this.Find(key);

        public IEnumerator<IGrouping<TKey, TValue>> GetEnumerator()
        {
            return new GroupingEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public struct Enumerable : IEnumerable<TValue>, IEnumerable
        {
            private readonly LinkedListNode<(TKey, TValue)> start;

            public Enumerable(LinkedListNode<(TKey, TValue)> start)
            {
                this.start = start;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new Enumerator(this.start);
            }

            public IEnumerator<TValue> GetEnumerator()
            {
                return new Enumerator(this.start);
            }
        }

        public struct Enumerator : IEnumerator<TValue>, IEnumerator
        {
            private LinkedListNode<(TKey, TValue)> node;
            private TKey originalKey;
            private readonly int version;
            private TValue current;

            internal Enumerator(LinkedListNode<(TKey, TValue)> node)
            {
                this.node = node;
                this.current = default;

                if (this.node == null)
                {
                    version = -1;
                    originalKey = default(TKey);
                }
                else
                {
                    version = node.List.version;
                    originalKey = node.Value.Item1;

                }
            }

            public TValue Current => current;

            object IEnumerator.Current => current;

            public bool MoveNext()
            {
                if (node == null)
                {
                    return false;
                }

                if (version != node.List.version)
                {
                    throw new InvalidOperationException("EnumFailedVersion");
                }

                if (!EqualityComparer<TKey>.Default.Equals(node.Value.Item1, originalKey))
                {
                    return false;
                }

                current = node.Value.Item2;
                node = node.Next;
                return true;
            }

            public void Reset()
            {
            }

            public void Dispose()
            {
            }
        }

        public struct GroupingEnumerator : IEnumerator<IGrouping<TKey, TValue>>, IEnumerator
        {
            private IGrouping<TKey, TValue> _current;
            private Dictionary<TKey, LinkedListNode<(TKey, TValue)>>.Enumerator bucketEnumerator;

            internal GroupingEnumerator(Lookup<TKey, TValue> lookup)
            {
                this._current = default;
                this.bucketEnumerator = lookup.bucketReference.GetEnumerator();
            }

            public IGrouping<TKey, TValue> Current => _current;

            object IEnumerator.Current => _current;

            public bool MoveNext()
            {
                var result = this.bucketEnumerator.MoveNext();
                if (result)
                {
                    _current = new Grouping(this.bucketEnumerator.Current.Key, this.bucketEnumerator.Current.Value);
                }
                return result;
            }

            public void Reset()
            {
                ((IEnumerator)this.bucketEnumerator).Reset();
            }

            public void Dispose()
            {
                this.bucketEnumerator.Dispose();
            }
        }

        private class Grouping : IGrouping<TKey, TValue>
        {
            private readonly LinkedListNode<(TKey, TValue)> node;

            public Grouping(TKey key, LinkedListNode<(TKey, TValue)> node)
            {
                this.Key = key;
                this.node = node;
            }

            public TKey Key { get; }

            public IEnumerator<TValue> GetEnumerator()
            {
                return new Enumerator(node);
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}