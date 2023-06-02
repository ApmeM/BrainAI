namespace BrainAI.Pathfinding
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// calculates paths given an IWeightedGraph and start/goal positions
    /// </summary>
    public class WeightedPathfinder<T> : IPathfinder<T>, IMultiTargetPathfinder<T>
    {
        private readonly Dictionary<T, T> visitedNodes = new Dictionary<T, T>();

        private readonly HashSet<T> tmpGoals = new HashSet<T>();

        private readonly IWeightedGraph<T> graph;

        private readonly List<T> resultPath = new List<T>();

        private readonly Dictionary<T, int> costSoFar = new Dictionary<T, int>();

        private readonly List<ValueTuple<int, T>> frontier = new List<ValueTuple<int, T>>();

        private static readonly Comparison<(int, T)> Comparison = (x, y) => x.Item1 - y.Item1;

        public WeightedPathfinder(IWeightedGraph<T> graph)
        {
            this.graph = graph;
        }

        public List<T> Search(T start, T goal)
        {
            tmpGoals.Clear();
            tmpGoals.Add(goal);
            return Search(start, tmpGoals);
        }

        public List<T> Search(T start, HashSet<T> goals)
        {
            var (target, result) = InnerSearch(start, goals, int.MaxValue);
            if (!result)
            {
                return null;
            }
            PathConstructor.RecontructPath(visitedNodes, start, target, resultPath);
            return resultPath;
        }

        public Dictionary<T, T> Search(T start, int maxPathWeight)
        {
            tmpGoals.Clear();
            InnerSearch(start, tmpGoals, maxPathWeight);
            return visitedNodes;
        }

        public Dictionary<T, T> Search(T start, HashSet<T> goals, int maxPathWeight)
        {
            InnerSearch(start, goals, maxPathWeight);
            return visitedNodes;
        }

        public ValueTuple<T, bool> InnerSearch(T start, HashSet<T> goals, int maxPathWeight)
        {
            visitedNodes.Clear();
            visitedNodes[start] = start;

            frontier.Clear();
            frontier.Add(new ValueTuple<int, T>(0, start));

            costSoFar.Clear();
            costSoFar[start] = 0;

            while (frontier.Count > 0)
            {
                var current = frontier[0];
                frontier.RemoveAt(0);

                if (current.Item1 >= maxPathWeight)
                {
                    break;
                }

                if (goals.Contains(current.Item2))
                {
                    return (current.Item2, true);
                }

                foreach (var next in graph.GetNeighbors(current.Item2))
                {
                    var newCost = costSoFar[current.Item2] + graph.Cost(current.Item2, next);
                    if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                    {
                        costSoFar[next] = newCost;
                        var priority = newCost;
                        frontier.Add(new ValueTuple<int, T>(priority, next));
                        visitedNodes[next] = current.Item2;
                    }
                }

                frontier.Sort(Comparison);
            }

            return (default(T), false);
        }
    }
}
