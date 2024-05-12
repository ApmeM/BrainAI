namespace BrainAI.Pathfinding
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Calculates paths given an IUnweightedGraph and start/goal positions
    /// </summary>
    public class BreadthFirstPathfinder<T> : IPathfinder<T>, ICoveragePathfinder<T>
    {
        public Dictionary<T, T> VisitedNodes { get; } = new Dictionary<T, T>();
        
        public List<T> ResultPath { get; set; } = new List<T>();

        private readonly HashSet<T> tmpGoals = new HashSet<T>();

        private T searchStart;

        private readonly IUnweightedGraph<T> graph;

        private readonly Queue<T> frontier = new Queue<T>();
        
        private readonly List<T> neighbours = new List<T>();

        public BreadthFirstPathfinder(IUnweightedGraph<T> graph)
        {
            this.graph = graph;
        }

        public void Search(T start, T goal)
        {
            this.PrepareSearch();
            this.StartNewSearch(start);

            tmpGoals.Add(goal);

            ContinueSearch();
        }

        public void Search(T start, HashSet<T> goals)
        {
            this.PrepareSearch();
            this.StartNewSearch(start);

            foreach (var goal in goals)
            {
                this.tmpGoals.Add(goal);
            }

            ContinueSearch();
        }

        public void Search(T start, int additionalDepth)
        {
            this.PrepareSearch();
            this.StartNewSearch(start);

            InternalSearch(additionalDepth);
        }

        public void Search(T start, T goal, int additionalDepth)
        {
            this.PrepareSearch();
            this.StartNewSearch(start);

            this.tmpGoals.Add(goal);

            ContinueSearch(additionalDepth);
        }

        public void Search(T start, HashSet<T> goals, int additionalDepth)
        {
            this.PrepareSearch();
            this.StartNewSearch(start);

            foreach (var goal in goals)
            {
                this.tmpGoals.Add(goal);
            }

            ContinueSearch(additionalDepth);
        }

        public void ContinueSearch()
        {
            if (tmpGoals.Count == 0)
            {
                return;
            }

            ContinueSearch(int.MaxValue);
        }

        public void ContinueSearch(int additionalDepth)
        {
            var (target, isFound) = InternalSearch(additionalDepth);
            if (isFound)
            {
                PathConstructor.RecontructPath(VisitedNodes, searchStart, target, this.ResultPath);
            }
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
                
                graph.GetNeighbors(current, neighbours);
                
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
    }
}
