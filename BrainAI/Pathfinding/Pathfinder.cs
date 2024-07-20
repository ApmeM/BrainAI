namespace BrainAI.Pathfinding
{
    using System;
    using System.Collections.Generic;

    public abstract class Pathfinder<T> : IPathfinder<T>
    {
        public Dictionary<T, T> VisitedNodes { get; } = new Dictionary<T, T>();

        public List<T> ResultPath { get; set; } = new List<T>();

        protected readonly HashSet<T> tmpGoals = new HashSet<T>();

        protected T searchStart;

        protected readonly Dictionary<T, int> costSoFar = new Dictionary<T, int>();

        internal readonly PriorityQueue<(int, T), int> frontier = new PriorityQueue<(int, T), int>();

        protected List<T> neighbours = new List<T>();

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
            this.ResultPath.Clear();
            if (tmpGoals.Count == 0)
            {
                return;
            }

            ContinueSearch(int.MaxValue);
        }

        public void ContinueSearch(int additionalDepth)
        {
            this.ResultPath.Clear();
            var (target, isFound) = InternalSearch(additionalDepth);
            if (isFound)
            {
                PathConstructor.RecontructPath(VisitedNodes, searchStart, target, this.ResultPath);
            }
        }

        protected void PrepareSearch()
        {
            this.ResultPath.Clear();
            this.frontier.Clear();
            this.VisitedNodes.Clear();
            this.tmpGoals.Clear();
            this.costSoFar.Clear();
        }

        protected void StartNewSearch(T start)
        {
            this.searchStart = start;
            this.VisitedNodes.Add(start, start);
            this.frontier.Enqueue(new ValueTuple<int, T>(0, start), 0);
            this.costSoFar[start] = 0;
        }

        internal abstract ValueTuple<T, bool> InternalSearch(int additionalDepth);
    }
}