namespace BrainAI.Pathfinding.AStar
{
    using BrainAI.Pathfinding.Dijkstra;

    /// <summary>
    /// interface for a graph that can be fed to the AStarPathfinder.search method
    /// </summary>
    public interface IAstarGraph<T> : IWeightedGraph<T>
    {
        /// <summary>
        /// calculates the heuristic (estimate) to get from 'node' to 'goal'. See WeightedGridGraph for the common Manhatten method.
        /// </summary>
        /// <param name="node">Node.</param>
        /// <param name="goal">Goal.</param>
        int Heuristic( T node, T goal );
    }
}

