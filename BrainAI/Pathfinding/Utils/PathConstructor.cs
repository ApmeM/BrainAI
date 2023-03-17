namespace BrainAI.Pathfinding
{
    using System.Collections.Generic;

    public class PathConstructor
    {
        public static void RecontructPath<T>(Dictionary<T, T> cameFrom, T start, T goal, List<T> path)
        {
            path.Clear();
            if (!cameFrom.ContainsKey(goal))
            {
                return;
            }

            var current = goal;
            path.Add(goal);

            while (!EqualityComparer<T>.Default.Equals(current, start))
            {
                current = cameFrom[current];
                path.Add(current);
            }
            path.Reverse();
        }
    }
}