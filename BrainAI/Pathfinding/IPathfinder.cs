namespace BrainAI.Pathfinding
{
    using System.Collections.Generic;

    public interface IPathfinder<T>
    {
        IReadOnlyList<T> Search(T start, T goal);
    }
}