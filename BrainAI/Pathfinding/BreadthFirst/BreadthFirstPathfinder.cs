namespace BrainAI.Pathfinding
{
    using System.Collections.Generic;

    /// <summary>
    /// calculates paths given an IUnweightedGraph and start/goal positions
    /// </summary>
    public class BreadthFirstPathfinder<T> : IPathfinder<T>
    {
        private readonly Dictionary<T, T> visitedNodes = new Dictionary<T, T>();

        private readonly IUnweightedGraph<T> graph;

        private readonly List<T> resultPath = new List<T>();

        private readonly Queue<T> frontier = new Queue<T>();

        public BreadthFirstPathfinder(IUnweightedGraph<T> graph)
        {
            this.graph = graph;
        }

        public IReadOnlyList<T> Search(T start, T goal)
        {
            frontier.Clear();
            frontier.Enqueue(start);

            visitedNodes.Clear();
            visitedNodes.Add(start, start);

            while (frontier.Count > 0)
            {
                var current = frontier.Dequeue();
                if (EqualityComparer<T>.Default.Equals(current, goal))
                {
                    PathConstructor.RecontructPath(visitedNodes, start, goal, resultPath);
                    return resultPath;
                }

                foreach (var next in graph.GetNeighbors(current))
                {
                    if (!visitedNodes.ContainsKey(next))
                    {
                        frontier.Enqueue(next);
                        visitedNodes.Add(next, current);
                    }
                }
            }

            return null;
        }

        public IReadOnlyList<T> Search(T start, HashSet<T> goals)
        {
            frontier.Clear();
            frontier.Enqueue(start);

            visitedNodes.Clear();
            visitedNodes.Add(start, start);

            while (frontier.Count > 0)
            {
                var current = frontier.Dequeue();
                if (goals.Contains(current))
                {
                    PathConstructor.RecontructPath(visitedNodes, start, current, resultPath);
                    return resultPath;
                }

                foreach (var next in graph.GetNeighbors(current))
                {
                    if (!visitedNodes.ContainsKey(next))
                    {
                        frontier.Enqueue(next);
                        visitedNodes.Add(next, current);
                    }
                }
            }
            
            return null;
        }

        public IReadOnlyDictionary<T, T> Search(T start, int length)
        {
            frontier.Clear();
            frontier.Enqueue(start);

            visitedNodes.Clear();
            visitedNodes.Add(start, start);

            var forNextLevel = 1;

            while (frontier.Count > 0 && length > 0)
            {
                var current = frontier.Dequeue();

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
                    length--;
                }
            }

            return visitedNodes;
        }
    }
}
