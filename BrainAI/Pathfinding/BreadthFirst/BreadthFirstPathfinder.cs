namespace BrainAI.Pathfinding.BreadthFirst
{
    using System.Collections.Generic;

    /// <summary>
    /// calculates paths given an IUnweightedGraph and start/goal positions
    /// </summary>
    public static class BreadthFirstPathfinder
    {
        public static bool Search<T>( IUnweightedGraph<T> graph, T start, T goal, out Dictionary<T,T> cameFrom )
        {
            var foundPath = false;
            var frontier = new Queue<T>();
            frontier.Enqueue( start );

            cameFrom = new Dictionary<T,T>();
            cameFrom.Add( start, start );

            while( frontier.Count > 0 )
            {
                var current = frontier.Dequeue();
                if( current.Equals( goal ) )
                {
                    foundPath = true;
                    break;
                }

                foreach( var next in graph.GetNeighbors( current ) )
                {
                    if( !cameFrom.ContainsKey( next ) )
                    {
                        frontier.Enqueue( next );
                        cameFrom.Add( next, current );
                    }
                }
            }

            return foundPath;
        }


        public static List<T> Search<T>( IUnweightedGraph<T> graph, T start, T goal )
        {
            var foundPath = Search( graph, start, goal, out var cameFrom );
            return foundPath ? PathConstructor.RecontructPath( cameFrom, start, goal ) : null;
        }

        public static bool Search<T>( IUnweightedGraph<T> graph, T start, HashSet<T> goals, out Dictionary<T,T> cameFrom )
        {
            var foundPath = false;
            var frontier = new Queue<T>();
            frontier.Enqueue( start );

            cameFrom = new Dictionary<T,T>();
            cameFrom.Add( start, start );

            while( frontier.Count > 0 )
            {
                var current = frontier.Dequeue();
                if(goals.Contains(current))
                {
                    foundPath = true;
                    break;
                }

                foreach( var next in graph.GetNeighbors( current ) )
                {
                    if( !cameFrom.ContainsKey( next ) )
                    {
                        frontier.Enqueue( next );
                        cameFrom.Add( next, current );
                    }
                }
            }

            return foundPath;
        }
        
        public static List<T> Search<T>( IUnweightedGraph<T> graph, T start, HashSet<T> goals )
        {
            var foundPath = Search( graph, start, goals, out var cameFrom );
            if (!foundPath)
            {
                return null;
            }

            foreach (var goal in goals)
            {
                if (cameFrom.ContainsKey(goal))
                {
                    return PathConstructor.RecontructPath(cameFrom, start, goal);
                }
            }

            return null;
        }


        public static void Search<T>(IUnweightedGraph<T> graph, T start, int length, out Dictionary<T, T> cameFrom)
        {
            var frontier = new Queue<T>();
            frontier.Enqueue(start);

            cameFrom = new Dictionary<T, T>();
            cameFrom.Add(start, start);

            var forNextLevel = 1;

            while (frontier.Count > 0)
            {
                var current = frontier.Dequeue();

                foreach (var next in graph.GetNeighbors(current))
                {
                    if (!cameFrom.ContainsKey(next))
                    {
                        frontier.Enqueue(next);
                        cameFrom.Add(next, current);
                    }
                }

                forNextLevel--;
                if (forNextLevel == 0)
                {
                    forNextLevel = frontier.Count;
                    length--;

                    if (length == 0)
                    {
                        break;
                    }
                }
            }
        }

    }
}

