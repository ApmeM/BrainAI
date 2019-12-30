namespace BrainAI.Pathfinding.AStar
{
    using System;
    using System.Collections.Generic;

    using BrainAI.Pathfinding.Utils;

    /// <summary>
    /// calculates paths given an IAstarGraph and start/goal positions
    /// </summary>
    public static class AStarPathfinder
    {
        public static bool Search<T>( IAstarGraph<T> graph, T start, T goal, out Dictionary<T,T> cameFrom )
        {
            var foundPath = false;
            cameFrom = new Dictionary<T, T> { { start, start } };

            var costSoFar = new Dictionary<T, int>();
            var frontier = new List<Tuple<int, T>> { new Tuple<int, T>(0, start) };

            costSoFar[start] = 0;

            while( frontier.Count > 0 )
            {
                var current = frontier[0];
                frontier.RemoveAt(0);

                if ( current.Item2.Equals( goal ) )
                {
                    foundPath = true;
                    break;
                }

                foreach( var next in graph.GetNeighbors( current.Item2) )
                {
                    var newCost = costSoFar[current.Item2] + graph.Cost( current.Item2, next );
                    if( !costSoFar.ContainsKey( next ) || newCost < costSoFar[next] )
                    {
                        costSoFar[next] = newCost;
                        var priority = newCost + graph.Heuristic( next, goal );
                        frontier.Add(new Tuple<int, T>(priority, next));
                        cameFrom[next] = current.Item2;
                    }
                }

                frontier.Sort(new TupleComparer<T>());
            }

            return foundPath;
        }
        
        /// <summary>
        /// gets a path from start to goal if possible. If no path is found null is returned.
        /// </summary>
        /// <param name="graph">Graph.</param>
        /// <param name="start">Start.</param>
        /// <param name="goal">Goal.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static List<T> Search<T>( IAstarGraph<T> graph, T start, T goal )
        {
            var foundPath = Search( graph, start, goal, out var cameFrom );
            return foundPath ? PathConstructor.RecontructPath( cameFrom, start, goal ) : null;
        }
    }
}

