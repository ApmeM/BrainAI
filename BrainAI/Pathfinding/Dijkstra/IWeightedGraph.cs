namespace BrainAI.Pathfinding.Dijkstra
{
    using BrainAI.Pathfinding.BreadthFirst;

    /// <summary>
    /// interface for a graph that can be fed to the DijkstraPathfinder.search method
    /// </summary>
    public interface IWeightedGraph<T> : IUnweightedGraph<T>
    {
        /// <summary>
        /// calculates the cost to get from 'from' to 'to'
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        int Cost( T from, T to );
    }
}

