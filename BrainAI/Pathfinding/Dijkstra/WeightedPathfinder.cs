namespace BrainAI.Pathfinding
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// calculates paths given an IWeightedGraph and start/goal positions
    /// </summary>
    public class WeightedPathfinder<T> : IPathfinder<T>, ICoveragePathfinder<T>
    {
        public Dictionary<T, T> VisitedNodes { get; } = new Dictionary<T, T>();

        private readonly HashSet<T> tmpGoals = new HashSet<T>();

        private T searchStart;

        private readonly IWeightedGraph<T> graph;

        private readonly List<T> resultPath = new List<T>();

        private readonly Dictionary<T, int> costSoFar = new Dictionary<T, int>();

        private readonly PriorityQueue<(int, T), int> frontier = new PriorityQueue<(int, T), int>();

        private static readonly Comparison<(int, T)> Comparison = (x, y) => x.Item1 - y.Item1;

        public WeightedPathfinder(IWeightedGraph<T> graph)
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

        public void Search(T start, int additionalDepth)
        {
            this.PrepareSearch();
            this.StartNewSearch(start);

            graph.BeforeSearch(start, tmpGoals);

            InternalSearch(additionalDepth);
        }
        
        public List<T> Search(T start, T goal, int additionalDepth)
        {
            this.PrepareSearch();
            this.StartNewSearch(start);

            this.tmpGoals.Add(goal);
            graph.BeforeSearch(start, tmpGoals);

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
            graph.BeforeSearch(start, tmpGoals);

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

                if (tmpGoals.Contains(current.Item2))
                {
                    tmpGoals.Remove(current.Item2);
                    return (current.Item2, true);
                }

                frontier.Dequeue();

                foreach (var next in graph.GetNeighbors(current.Item2))
                {
                    var newCost = costSoFar[current.Item2] + graph.Cost(current.Item2, next);
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

        private void PrepareSearch()
        {
            this.frontier.Clear();
            this.VisitedNodes.Clear();
            this.tmpGoals.Clear();
            this.costSoFar.Clear();
        }

        private void StartNewSearch(T start)
        {
            this.searchStart = start;
            this.frontier.Enqueue(new ValueTuple<int, T>(0, start), 0);
            this.VisitedNodes.Add(start, start);
            this.costSoFar[start] = 0;
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
