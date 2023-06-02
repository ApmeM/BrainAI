namespace BrainAI.Pathfinding
{
    using System.Collections.Generic;

    public interface IMultiTargetPathfinder<T> : IPathfinder<T>
    {
        List<T> Search(T start, HashSet<T> goals);
        
        Dictionary<T, T> Search(T start, int maxPathWeight);

        Dictionary<T, T> Search(T start, HashSet<T> goals, int maxPathWeight);
    }
}