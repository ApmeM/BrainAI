namespace BrainAI.Pathfinding
{
    using System.Collections.Generic;

    /// <summary>
    /// interface for a graph that can be fed to the BreadthFirstPathfinder.search method
    /// </summary>
    public interface IUnweightedGraph<T>
    {
        /// <summary>
        /// The GetNeighbors method should return any neighbor nodes that can be reached from the passed in node.
        /// </summary>
        /// <returns>The neighbors.</returns>
        /// <param name="node">Node.</param>
        void GetNeighbors( T node, ref List<T> result );
    }
}

