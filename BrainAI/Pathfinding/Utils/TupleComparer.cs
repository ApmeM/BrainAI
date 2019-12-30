namespace BrainAI.Pathfinding.Utils
{
    using System;
    using System.Collections.Generic;

    internal class TupleComparer<T> : IComparer<Tuple<int, T>>
    {
        public int Compare(Tuple<int, T> x, Tuple<int, T> y)
        {
            return x.Item1 - y.Item1;
        }
    }
}