using System.Collections.Generic;
using System;
using System.Collections;
using System.Linq;

namespace BrainAI.Pathfinding
{
    /// <summary>
    /// Zero memory allocation lookup.
    /// Copied from https://github.com/ApmeM/FingerMath/blob/master/FingerMath/Collections/Lookup.cs
    /// </summary>
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
                count++;

                if (ignoreDuplicates)
                {
                    set.Add(tuple);
                }
            }

            version++;
        }

        public void Remove(TKey key)
        {
            while (startReference.ContainsKey(key))
            {
                Remove(key, startReference[key].Value.Item2);
            }
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
                !EqualityComparer<TValue>.Default.Equals(start.Value.Item2, value) &&
                !EqualityComparer<LinkedListNode<(TKey, TValue)>>.Default.Equals(start, endReference[key]))
            {
                start = start.next;
            }

            if (
                !EqualityComparer<TKey>.Default.Equals(start.Value.Item1, key) ||
                !EqualityComparer<TValue>.Default.Equals(start.Value.Item2, value)
            )
            {
                return;
            }

            counts[key]--;

            if (start == startReference[key] && start == endReference[key])
            {
                startReference.Remove(key);
                endReference.Remove(key);
                counts.Remove(key);
                count--;

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

        public bool Contains(TKey key)
        {
            return startReference.ContainsKey(key);
        }

        public Enumerable this[TKey key] => this.Find(key);

        public GroupingEnumerator GetEnumerator() => new GroupingEnumerator(this);

        private Enumerable Find(TKey key) => new Enumerable(this, key);

        [Obsolete("Use Find instead. This method requires boxing and allocates memory.")]
        IEnumerable<TValue> ILookup<TKey, TValue>.this[TKey key] => this.Find(key);

        [Obsolete("Use GetEnumerator instead. This method requires boxing and allocates memory.")]
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        [Obsolete("Use GetEnumerator instead. This method requires boxing and allocates memory.")]
        IEnumerator<IGrouping<TKey, TValue>> IEnumerable<IGrouping<TKey, TValue>>.GetEnumerator() => this.GetEnumerator();

        public struct Enumerable : IEnumerable<TValue>, IEnumerable, IGrouping<TKey, TValue>, ICollection<TValue>
        {
            private readonly Lookup<TKey, TValue> lookup;
            public TKey Key { get; }

            public int Count => this.lookup.counts.ContainsKey(this.Key) ? this.lookup.counts[this.Key] : 0;

            public bool IsReadOnly => false;

            public Enumerable(Lookup<TKey, TValue> lookup, TKey key)
            {
                this.lookup = lookup;
                this.Key = key;
            }

            [Obsolete("Use GetEnumerator instead. This method requires boxing and allocates memory.")]
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            [Obsolete("Use GetEnumerator instead. This method requires boxing and allocates memory.")]
            IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator() => GetEnumerator();

            public Enumerator GetEnumerator() => new Enumerator(this.lookup, this.Key);

            public void Add(TValue item)
            {
                this.lookup.Add(this.Key, item);
            }

            public void Clear()
            {
                this.lookup.Remove(this.Key);
            }

            public bool Contains(TValue item)
            {
                foreach (var el in this)
                {
                    if (EqualityComparer<TValue>.Default.Equals(el, item))
                    {
                        return true;
                    }
                }

                return false;
            }

            public void CopyTo(TValue[] array, int arrayIndex)
            {
                foreach (var el in this)
                {
                    array[arrayIndex] = el;
                }
            }

            public bool Remove(TValue item)
            {
                this.lookup.Remove(this.Key, item);
                return true;
            }
        }

        public struct Enumerator : IEnumerator<TValue>, IEnumerator
        {
            private readonly int version;
            private readonly Lookup<TKey, TValue> lookup;
            private readonly TKey key;
            private LinkedListNode<(TKey, TValue)> node;
            private TValue current;

            public TValue Current => current;

            object IEnumerator.Current => current;

            internal Enumerator(Lookup<TKey, TValue> lookup, TKey key)
            {
                this.current = default;
                this.version = lookup.version;
                this.lookup = lookup;
                this.key = key;
                this.node = this.lookup.startReference.ContainsKey(key) ? this.lookup.startReference[key] : null;
            }

            public bool MoveNext()
            {
                if (node == null)
                {
                    return false;
                }

                if (version != lookup.version)
                {
                    throw new InvalidOperationException("The underlying collection was changed.");
                }

                current = node.Value.Item2;

                if (node == this.lookup.endReference[key])
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
            private int version;
            private Dictionary<TKey, LinkedListNode<(TKey, TValue)>>.Enumerator bucketEnumerator;

            internal GroupingEnumerator(Lookup<TKey, TValue> lookup)
            {
                this.Current = default;
                this.lookup = lookup;
                this.version = this.lookup.version;
                this.bucketEnumerator = lookup.startReference.GetEnumerator();
            }

            public Enumerable Current { get; set; }

            [Obsolete("Use Current instead. This method requires boxing and allocates memory.")]
            object IEnumerator.Current => Current;

            [Obsolete("Use Current instead. This method requires boxing and allocates memory.")]
            IGrouping<TKey, TValue> IEnumerator<IGrouping<TKey, TValue>>.Current => Current;

            public bool MoveNext()
            {
                if (version != this.lookup.version)
                {
                    throw new InvalidOperationException("The underlying collection was changed.");
                }

                var result = this.bucketEnumerator.MoveNext();
                if (result)
                {
                    var key = this.bucketEnumerator.Current.Key;
                    Current = new Enumerable(this.lookup, key);
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