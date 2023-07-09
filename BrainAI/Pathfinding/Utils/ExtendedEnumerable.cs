using System.Collections.Generic;
using System.Collections;
using System;

namespace BrainAI.Pathfinding
{
    public struct ExtendedEnumerable<T> : IEnumerable<T>
    {
        private readonly List<T> baseEnumerable;
        private readonly int total;

        public ExtendedEnumerable(List<T> baseEnumerable, int total)
        {
            this.baseEnumerable = baseEnumerable;
            this.total = total;
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(baseEnumerable, total);
        }

        [Obsolete("Use GetEnumerator instead. This method requires boxing and allocates memory.")]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [Obsolete("Use GetEnumerator instead. This method requires boxing and allocates memory.")]
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        public struct Enumerator : IEnumerator<T>
        {
            private List<T>.Enumerator baseEnumerator;
            private List<T>.Enumerator enumerator;
            public T Current { get; set; }
            private int total;

            public Enumerator(List<T> baseEnumerable, int total)
            {
                this.baseEnumerator = baseEnumerable.GetEnumerator();
                this.enumerator = baseEnumerable.GetEnumerator();
                this.total = total;

                this.Current = default(T);
            }


            [Obsolete("Use Current instead. This method requires boxing and allocates memory.")]
            object IEnumerator.Current => Current;

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (total == 0)
                {
                    return false;
                }
                total--;

                var result = this.baseEnumerator.MoveNext();
            tryAgain:
                if (!result)
                {
                    this.baseEnumerator = enumerator;
                    result = this.baseEnumerator.MoveNext();
                    if (!result)
                    {
                        return false;
                    }
                    goto tryAgain;
                }

                Current = this.baseEnumerator.Current;
                return true;
            }

            public void Reset()
            {
            }
        }
    }
}