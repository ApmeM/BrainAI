namespace BrainAI.Pathfinding
{
    using System.Collections.Generic;

    /// <summary>
    /// interface for a graph that handles start and end search processes.
    /// </summary>
    public interface ISearchableGraph<T>
    {
        /// <summary>
        /// Executed before search
        /// </summary>
        void BeforeSearch( T nodeStart, HashSet<T> nodeEnd );
    
    }
}

