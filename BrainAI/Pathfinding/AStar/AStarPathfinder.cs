namespace BrainAI.Pathfinding
{
    using System;

    public class AStarPathfinder<T> : Pathfinder<T>
    {
        private readonly IAstarGraph<T> graph;

        public AStarPathfinder(IAstarGraph<T> graph)
        {
            this.graph = graph;
        }

        internal override ValueTuple<T, bool> InternalSearch(int additionalDepth)
        {
            var goal = this.GetFirstGoal();

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

                graph.GetNeighbors(current.Item2, neighbours);

                foreach (var next in neighbours)
                {
                    var newCost = costSoFar[current.Item2] + graph.Cost(current.Item2, next);
                    if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                    {
                        costSoFar[next] = newCost;
                        var priority = newCost + graph.Heuristic(next, goal);
                        frontier.Enqueue(new ValueTuple<int, T>(priority, next), priority);
                        VisitedNodes[next] = current.Item2;
                    }
                }
            }

            return (default(T), false);
        }

        private T GetFirstGoal()
        {
            foreach (var g in tmpGoals)
            {
                return g;
            }

            throw new Exception("No goals found.");
        }
    }
}
