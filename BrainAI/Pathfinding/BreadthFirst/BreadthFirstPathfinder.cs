namespace BrainAI.Pathfinding
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// calculates paths given an IUnweightedGraph and start/goal positions
    /// </summary>
    public class BreadthFirstPathfinder<T> : IPathfinder<T>, IMultiTargetPathfinder<T>
    {
        private readonly Dictionary<T, T> visitedNodes = new Dictionary<T, T>();

        private readonly HashSet<T> tmpGoals = new HashSet<T>();

        private readonly IUnweightedGraph<T> graph;

        private readonly List<T> resultPath = new List<T>();

        private readonly Queue<T> frontier = new Queue<T>();

        public BreadthFirstPathfinder(IUnweightedGraph<T> graph)
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
            frontier.Clear();
            frontier.Enqueue(start);

            visitedNodes.Clear();
            visitedNodes.Add(start, start);

            var forNextLevel = 1;

            while (frontier.Count > 0 && maxPathWeight > 0)
            {
                var current = frontier.Dequeue();

                if (goals.Contains(current))
                {
                    return (current, true);
                }

                foreach (var next in graph.GetNeighbors(current))
                {
                    if (!visitedNodes.ContainsKey(next))
                    {
                        frontier.Enqueue(next);
                        visitedNodes.Add(next, current);
                    }
                }

                forNextLevel--;
                if (forNextLevel == 0)
                {
                    forNextLevel = frontier.Count;
                    maxPathWeight--;
                }
            }

            return (default(T), false);
        }
    }
}
