namespace BrainAI.Pathfinding.BreadthFirst
{
    using System.Collections.Generic;

    /// <summary>
    /// basic implementation of an UnweightedGraph. All edges are cached. This type of graph is best suited for non-grid based graphs.
    /// Any nodes added as edges must also have an entry as the key in the edges Dictionary.
    /// </summary>
    public class UnweightedGraph<T> : IUnweightedGraph<T>
    {
        public Dictionary<T,T[]> Edges = new Dictionary<T,T[]>();

        public IEnumerable<T> GetNeighbors( T node )
        {
            return this.Edges[node];
        }
    }
}

