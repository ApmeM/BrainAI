namespace BrainAI.Pathfinding
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface for a graph that can be fed to the BreadthFirstPathfinder.search method
    /// </summary>
    public interface IUnweightedGraph<T>
    {
        /// <summary>
        /// The GetNeighbors method should add any neighbor nodes that can be reached from the input node to the result collection.
        /// </summary>
        void GetNeighbors( T node, ICollection<T> result );
    }
}

