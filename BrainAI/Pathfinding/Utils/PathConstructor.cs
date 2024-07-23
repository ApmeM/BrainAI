namespace BrainAI.Pathfinding
{
    using System.Collections.Generic;

    public class PathConstructor
    {
        /// <summary>
        /// Construct path from start to goal using visited nodes. The result is cleared and set with the constructed path.
        /// </summary>
        public static void RecontructPath<T>(Dictionary<T, T> visitedNodes, T start, T goal, ICollection<T> result)
        {
            result.Clear();
            if (!visitedNodes.ContainsKey(goal))
            {
                return;
            }

            RecontructPathRecursive(visitedNodes, start, goal, result);
        }

        public static void RecontructPathRecursive<T>(Dictionary<T, T> visitedNodes, T start, T current, ICollection<T> result)
        {
            if (EqualityComparer<T>.Default.Equals(current, start))
            {
                result.Add(current);
                return;
            }
            
            RecontructPathRecursive(visitedNodes, start, visitedNodes[current], result);
            result.Add(current);
        }
    }
}