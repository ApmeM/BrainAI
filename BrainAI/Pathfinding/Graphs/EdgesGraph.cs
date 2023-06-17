namespace BrainAI.Pathfinding
{
    using System.Collections.Generic;

    /// <summary>
    /// Basic implementation Graph with edges. All edges are cached. This type of graph is best suited for non-grid based graphs.
    /// Any nodes added as edges must also have an entry as the key in the edges Dictionary.
    /// </summary>
    public class EdgesGraph<T> : IWeightedGraph<T>
    {
        public Dictionary<T, List<T>> Edges = new Dictionary<T, List<T>>();
        public Dictionary<(T, T), int> Weights = new Dictionary<(T, T), int>();

        public int DefaultWeight = 1;

        public List<T> GetNeighbors(T node)
        {
            return this.Edges[node];
        }

        public int Cost(T from, T to)
        {
            return this.Weights.ContainsKey((from, to)) ? this.Weights[(from, to)] : this.DefaultWeight;
        }

        public void BeforeSearch(T nodeStart, HashSet<T> nodeEnd)
        {
        }
    }
}

