namespace BrainAI.Pathfinding
{
    using System.Collections.Generic;

    public class PathConstructor
    {
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