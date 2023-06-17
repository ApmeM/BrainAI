namespace BrainAI.Pathfinding
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// calculates paths given an IUnweightedGraph and start/goal positions
    /// </summary>
    public class BreadthFirstPathfinder<T> : IPathfinder<T>, ICoveragePathfinder<T>
    {
        public Dictionary<T, T> VisitedNodes { get; } = new Dictionary<T, T>();

        private readonly HashSet<T> tmpGoals = new HashSet<T>();

        private T searchStart;

        private readonly IUnweightedGraph<T> graph;

        private readonly List<T> resultPath = new List<T>();

        private readonly Queue<T> frontier = new Queue<T>();

        public BreadthFirstPathfinder(IUnweightedGraph<T> graph)
        {
            this.graph = graph;
        }

        public List<T> Search(T start, T goal)
        {
            this.PrepareSearch();
            this.StartNewSearch(start);

            tmpGoals.Add(goal);
            graph.BeforeSearch(start, tmpGoals);

            return ContinueSearch();
        }

        public List<T> Search(T start, HashSet<T> goals)
        {
            this.PrepareSearch();
            this.StartNewSearch(start);

            foreach (var goal in goals)
            {
                this.tmpGoals.Add(goal);
            }
            graph.BeforeSearch(start, tmpGoals);

            return ContinueSearch();
        }

        public void Search(T start, int maxPathWeight)
        {
            this.PrepareSearch();
            this.StartNewSearch(start);

            graph.BeforeSearch(start, tmpGoals);

            InternalSearch(maxPathWeight);
        }

        public List<T> Search(T start, HashSet<T> goals, int maxPathWeight)
        {
            this.PrepareSearch();
            this.StartNewSearch(start);

            foreach (var goal in goals)
            {
                this.tmpGoals.Add(goal);
            }
            graph.BeforeSearch(start, tmpGoals);

            return ContinueSearch(maxPathWeight);
        }

        public List<T> ContinueSearch()
        {
            if (tmpGoals.Count == 0)
            {
                return null;
            }

            return ContinueSearch(int.MaxValue);
        }

        public List<T> ContinueSearch(int maxPathWeight)
        {
            var (target, result) = InternalSearch(maxPathWeight);
            return this.BuildPath(target, result);
        }

        private ValueTuple<T, bool> InternalSearch(int additionalDepth)
        {
            var forNextLevel = frontier.Count;

            while (frontier.Count > 0 && additionalDepth > 0)
            {
                var current = frontier.Peek();

                if (tmpGoals.Contains(current))
                {
                    tmpGoals.Remove(current);
                    return (current, true);
                }

                frontier.Dequeue();

                foreach (var next in graph.GetNeighbors(current))
                {
                    if (!VisitedNodes.ContainsKey(next))
                    {
                        frontier.Enqueue(next);
                        VisitedNodes.Add(next, current);
                    }
                }

                forNextLevel--;
                if (forNextLevel == 0)
                {
                    forNextLevel = frontier.Count;
                    additionalDepth--;
                }
            }

            return (default(T), false);
        }

        private void PrepareSearch()
        {
            this.frontier.Clear();
            this.VisitedNodes.Clear();
            this.tmpGoals.Clear();
        }

        private void StartNewSearch(T start)
        {
            this.searchStart = start;
            this.frontier.Enqueue(start);
            this.VisitedNodes.Add(start, start);
        }

        private List<T> BuildPath(T target, bool result)
        {
            if (!result)
            {
                return null;
            }

            PathConstructor.RecontructPath(VisitedNodes, searchStart, target, resultPath);
            return resultPath;
        }
    }
}
