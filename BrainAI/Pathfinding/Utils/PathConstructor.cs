namespace BrainAI.Pathfinding
{
    using System.Collections.Generic;

    public class PathConstructor
    {
        /// <summary>
        /// Construct path from start to goal using visited nodes. The result is cleared and set with the constructed path.
        /// </summary>
        public static void RecontructPath<T>(Dictionary<T, T> visitedNodes, T start, T goal, List<T> result)
        {
            result.Clear();
            if (!visitedNodes.ContainsKey(goal))
            {
                return;
            }

            var current = goal;
            result.Add(goal);

            while (!EqualityComparer<T>.Default.Equals(current, start))
            {
                current = visitedNodes[current];
                result.Add(current);
            }
            result.Reverse();
        }
    }
}