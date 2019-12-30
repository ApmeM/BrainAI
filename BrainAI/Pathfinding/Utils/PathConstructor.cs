namespace BrainAI.Pathfinding
{
    using System.Collections.Generic;

    internal class PathConstructor
    {
        /// <summary>
        /// reconstructs a path from the cameFrom Dictionary
        /// </summary>
        /// <returns>The path.</returns>
        /// <param name="cameFrom">Came from.</param>
        /// <param name="start">Start.</param>
        /// <param name="goal">Goal.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static List<T> RecontructPath<T>(Dictionary<T, T> cameFrom, T start, T goal)
        {
            var path = new List<T>();
            var current = goal;
            path.Add(goal);

            while (!current.Equals(start))
            {
                current = cameFrom[current];
                path.Add(current);
            }
            path.Reverse();

            return path;
        }
    }
}