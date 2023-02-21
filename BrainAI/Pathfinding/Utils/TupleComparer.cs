namespace BrainAI.Pathfinding
{
    using System;
    using System.Collections.Generic;

    internal class TupleComparer<T> : IComparer<ValueTuple<int, T>>
    {
        public int Compare(ValueTuple<int, T> x, ValueTuple<int, T> y)
        {
            return x.Item1 - y.Item1;
        }
    }
}