namespace BrainAI.Pathfinding
{
    using System.Collections.Generic;

    public interface IPathfinder<T>
    {
        List<T> Search(T start, T goal);
    }
}