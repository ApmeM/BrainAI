namespace BrainAI.Pathfinding
{
    using System;

    /// <summary>
    /// Calculates paths given an IUnweightedGraph and start/goal positions
    /// </summary>
    public class BreadthFirstPathfinder<T> : CoveragePathfinder<T>
    {
        private readonly IUnweightedGraph<T> graph;

        public BreadthFirstPathfinder(IUnweightedGraph<T> graph)
        {
            this.graph = graph;
        }

        internal override ValueTuple<T, bool> InternalSearch(int additionalDepth)
        {
            while (frontier.Count > 0 && additionalDepth > 0)
            {
                additionalDepth--;
                var current = frontier.Peek();

                if (tmpGoals.Contains(current.Item2))
                {
                    tmpGoals.Remove(current.Item2);
                    return (current.Item2, true);
                }

                frontier.Dequeue();

                neighbours.Clear();
                graph.GetNeighbors(current.Item2, neighbours);

                foreach (var next in neighbours)
                {
                    var newCost = costSoFar[current.Item2] + 1;
                    if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                    {
                        costSoFar[next] = newCost;
                        var priority = newCost;
                        frontier.Enqueue(new ValueTuple<int, T>(priority, next), priority);
                        VisitedNodes[next] = current.Item2;
                    }
                }
            }

            return (default(T), false);
        }
    }
}
