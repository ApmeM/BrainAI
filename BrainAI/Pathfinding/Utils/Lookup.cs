using System.Collections.Generic;
using System;
using System.Collections;
using System.Linq;
using System.Diagnostics;

namespace BrainAI.Pathfinding
{
    public class Lookup<TKey, TValue> : ILookup<TKey, TValue>
    {
        private Dictionary<TKey, LinkedListNode<(TKey, TValue)>> startReference = new Dictionary<TKey, LinkedListNode<(TKey, TValue)>>();
        private Dictionary<TKey, LinkedListNode<(TKey, TValue)>> endReference = new Dictionary<TKey, LinkedListNode<(TKey, TValue)>>();
        private Dictionary<TKey, int> counts = new Dictionary<TKey, int>();
        private LinkedList<(TKey, TValue)> valuesList = new LinkedList<(TKey, TValue)>();
        private HashSet<(TKey, TValue)> set = new HashSet<(TKey, TValue)>();

        private int count;
        private int version;
        private readonly bool ignoreDuplicates;

        public Lookup(bool ignoreDuplicates = false)
        {
            this.ignoreDuplicates = ignoreDuplicates;
        }

        public int Count
        {
            get { return count; }
        }

        public void Add(TKey key, TValue value)
        {
            var tuple = (key, value);
            if (startReference.ContainsKey(key))
            {
                if (ignoreDuplicates)
                {
                    if (set.Contains(tuple))
                    {
                        return;
                    }
                    else
                    {
                        set.Add(tuple);
                    }
                }
                valuesList.AddAfter(endReference[key], tuple);
                endReference[key] = endReference[key].Next;
                counts[key]++;
            }
            else
            {
                var node = valuesList.AddLast(tuple);
                startReference[key] = node;
                endReference[key] = node;
                counts[key] = 1;
                if (ignoreDuplicates)
                {
                    set.Add(tuple);
                }
            }
            version++;
            count++;
        }

        public void Remove(TKey key, TValue value)
        {
            if (!startReference.ContainsKey(key))
            {
                return;
            }

            var start = startReference[key];
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

            counts[key]--;

            if (start == startReference[key] && start == endReference[key])
            {
                startReference.Remove(key);
                endReference.Remove(key);
                counts.Remove(key);
            }
            else if (start == startReference[key])
            {
                startReference[key] = startReference[key].Next;
            }
            else if (start == endReference[key])
            {
                endReference[key] = endReference[key].Previous;
            }
            
            if (ignoreDuplicates)
            {
                set.Remove((key, value));
            }

            valuesList.Remove(start);
            version++;
            count--;
        }

        public void Clear()
        {
            valuesList.Clear();
            startReference.Clear();
            endReference.Clear();
            counts.Clear();
            set.Clear();
            count = 0;
            version++;
        }

        public Enumerable Find(TKey key)
        {
            if (!startReference.ContainsKey(key))
            {
                return new Enumerable(null, null, 0);
            }

            return new Enumerable(startReference[key], endReference[key], counts[key]);
        }

        public bool Contains(TKey key)
        {
            return startReference.ContainsKey(key);
        }

        [Obsolete("Use Find instead. This method requires boxing and allocates memory.")]
        public IEnumerable<TValue> this[TKey key] => this.Find(key);

        public GroupingEnumerator GetEnumerator()
        {
            return new GroupingEnumerator(this);
        }

        [Obsolete("Use GetEnumerator instead. This method requires boxing and allocates memory.")]
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        [Obsolete("Use GetEnumerator instead. This method requires boxing and allocates memory.")]
        IEnumerator<IGrouping<TKey, TValue>> IEnumerable<IGrouping<TKey, TValue>>.GetEnumerator() => this.GetEnumerator();

        public struct Enumerable : IEnumerable<TValue>, IEnumerable, IGrouping<TKey, TValue>
        {
            private readonly LinkedListNode<(TKey, TValue)> start;
            private readonly LinkedListNode<(TKey, TValue)> end;
            public readonly int Count;

            public TKey Key => start.Value.Item1;

            public Enumerable(LinkedListNode<(TKey, TValue)> start, LinkedListNode<(TKey, TValue)> end, int count)
            {
                this.start = start;
                this.end = end;
                this.Count = count;
            }

            [Obsolete("Use GetEnumerator instead. This method requires boxing and allocates memory.")]
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            [Obsolete("Use GetEnumerator instead. This method requires boxing and allocates memory.")]
            IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator() => GetEnumerator();

            public Enumerator GetEnumerator()
            {
                return new Enumerator(this.start, this.end);
            }
        }

        public struct Enumerator : IEnumerator<TValue>, IEnumerator
        {
            private LinkedListNode<(TKey, TValue)> node;
            private readonly int version;
            private readonly LinkedListNode<(TKey, TValue)> start;
            private LinkedListNode<(TKey, TValue)> end;
            private TValue current;

            public TValue Current => current;

            object IEnumerator.Current => current;

            internal Enumerator(LinkedListNode<(TKey, TValue)> start, LinkedListNode<(TKey, TValue)> end)
            {
                this.start = start;
                this.end = end;
                this.node = start;
                this.current = default;

                if (this.node == null)
                {
                    version = -1;
                }
                else
                {
                    version = start.List.version;
                }
            }

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

                current = node.Value.Item2;

                if (node == end)
                {
                    node = null;
                }
                else
                {
                    node = node.Next;
                }

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
            private Lookup<TKey, TValue> lookup;
            private Dictionary<TKey, LinkedListNode<(TKey, TValue)>>.Enumerator bucketEnumerator;

            internal GroupingEnumerator(Lookup<TKey, TValue> lookup)
            {
                this.Current = default;
                this.lookup = lookup;
                this.bucketEnumerator = lookup.startReference.GetEnumerator();
            }

            public Enumerable Current;

            [Obsolete("Use Current instead. This method requires boxing and allocates memory.")]
            object IEnumerator.Current => Current;

            [Obsolete("Use Current instead. This method requires boxing and allocates memory.")]
            IGrouping<TKey, TValue> IEnumerator<IGrouping<TKey, TValue>>.Current => Current;

            public bool MoveNext()
            {
                var result = this.bucketEnumerator.MoveNext();
                if (result)
                {
                    var key = this.bucketEnumerator.Current.Key;
                    Current = new Enumerable(this.lookup.startReference[key], this.lookup.endReference[key], this.lookup.counts[key]);
                }
                return result;
            }

            public void Reset()
            {
                // ((IEnumerator)this.bucketEnumerator).Reset();
            }

            public void Dispose()
            {
                this.bucketEnumerator.Dispose();
            }
        }
    }
}