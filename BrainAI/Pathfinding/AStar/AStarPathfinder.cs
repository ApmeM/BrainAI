namespace BrainAI.Pathfinding
{
    using System;
    using System.Linq;
    using System.Collections.Generic;

    public class AStarPathfinder<T> : IPathfinder<T>
    {
        public Dictionary<T, T> VisitedNodes { get; } = new Dictionary<T, T>();

        private readonly HashSet<T> tmpGoals = new HashSet<T>();

        private T searchStart;

        private readonly IAstarGraph<T> graph;

        private readonly List<T> resultPath = new List<T>();

        private readonly Dictionary<T, int> costSoFar = new Dictionary<T, int>();

        private readonly PriorityQueue<(int, T), int> frontier = new PriorityQueue<(int, T), int>();

        private static readonly Comparison<(int, T)> Comparison = (x, y) => x.Item1 - y.Item1;

        public AStarPathfinder(IAstarGraph<T> graph)
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

        /// AStar is not really multigoal search as it have a heuristics calculations based on a single target.
        /// Instead it took first goal from set and tries to get to it. 
        /// Each ContinueSearch will select next goal from set if previous was reached.
        public List<T> Search(T start, T goal, int additionalDepth)
        {
            this.PrepareSearch();
            this.StartNewSearch(start);

            this.tmpGoals.Add(goal);

            return ContinueSearch(additionalDepth);
        }

        /// AStar is not really multigoal search as it have a heuristics calculations based on a single target.
        /// Instead it took first goal from set and tries to get to it. 
        /// Each ContinueSearch will select next goal from set if previous was reached.
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

                foreach (var next in graph.GetNeighbors(current.Item2))
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
            this.VisitedNodes.Add(start, start);
            this.frontier.Enqueue(new ValueTuple<int, T>(0, start), 0);
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
