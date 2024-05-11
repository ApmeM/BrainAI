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
        private List<T> neighbours;

        public BreadthFirstPathfinder(IUnweightedGraph<T> graph)
        {
            this.graph = graph;
        }

        public List<T> Search(T start, T goal)
        {
            this.PrepareSearch();
            this.StartNewSearch(start);

            tmpGoals.Add(goal);

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

            return ContinueSearch();
        }

        public void Search(T start, int additionalDepth)
        {
            this.PrepareSearch();
            this.StartNewSearch(start);

            InternalSearch(additionalDepth);
        }

        public List<T> Search(T start, T goal, int additionalDepth)
        {
            this.PrepareSearch();
            this.StartNewSearch(start);

            this.tmpGoals.Add(goal);

            return ContinueSearch(additionalDepth);
        }

        public List<T> Search(T start, HashSet<T> goals, int additionalDepth)
        {
            this.PrepareSearch();
            this.StartNewSearch(start);

            foreach (var goal in goals)
            {
                this.tmpGoals.Add(goal);
            }

            return ContinueSearch(additionalDepth);
        }

        public List<T> ContinueSearch()
        {
            if (tmpGoals.Count == 0)
            {
                return null;
            }

            return ContinueSearch(int.MaxValue);
        }

        public List<T> ContinueSearch(int additionalDepth)
        {
            var (target, result) = InternalSearch(additionalDepth);
            return this.BuildPath(target, result);
        }

        private ValueTuple<T, bool> InternalSearch(int additionalDepth)
        {
            while (frontier.Count > 0 && additionalDepth > 0)
            {
                additionalDepth--;
                var current = frontier.Peek();

                if (tmpGoals.Contains(current))
                {
                    tmpGoals.Remove(current);
                    return (current, true);
                }

                frontier.Dequeue();
                
                graph.GetNeighbors(current, ref neighbours);
                
                foreach (var next in neighbours)
                {
                    if (!VisitedNodes.ContainsKey(next))
                    {
                        frontier.Enqueue(next);
                        VisitedNodes.Add(next, current);
                    }
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
