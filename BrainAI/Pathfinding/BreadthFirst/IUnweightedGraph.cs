﻿namespace BrainAI.Pathfinding
{
    using System.Collections.Generic;

    /// <summary>
    /// interface for a graph that can be fed to the BreadthFirstPathfinder.search method
    /// </summary>
    public interface IUnweightedGraph<T> : ISearchableGraph<T>
    {
        /// <summary>
        /// The getNeighbors method should return any neighbor nodes that can be reached from the passed in node.
        /// </summary>
        /// <returns>The neighbors.</returns>
        /// <param name="node">Node.</param>
        List<T> GetNeighbors( T node );
    }
}

