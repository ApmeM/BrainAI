namespace BrainAI.Pathfinding
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// calculates paths given an IWeightedGraph and start/goal positions
    /// </summary>
    public class WeightedPathfinder<T>
    {
        private readonly Dictionary<T,T> visitedNodes = new Dictionary<T, T>();

        private readonly IWeightedGraph<T> graph;

        private readonly List<T> resultPath = new List<T>();
        
        private readonly Dictionary<T, int> costSoFar = new Dictionary<T, int>();

        private readonly List<ValueTuple<int, T>> frontier = new List<ValueTuple<int, T>>();

        private readonly TupleComparer<T> comparer = new TupleComparer<T>();

        public WeightedPathfinder(IWeightedGraph<T> graph)
        {
            this.graph = graph;
        }

        public IReadOnlyList<T> Search(T start, T goal)
        {
            visitedNodes.Clear();
            visitedNodes[start] = start;

            frontier.Clear();
            frontier.Add(new ValueTuple<int, T>(0, start));

            costSoFar.Clear();
            costSoFar[start] = 0;

            while( frontier.Count > 0 )
            {
                var current = frontier[0];
                frontier.RemoveAt(0);

                if ( current.Item2.Equals( goal ) )
                {
                    PathConstructor.RecontructPath(visitedNodes, start, goal, resultPath);
                    return resultPath;
                }

                foreach( var next in graph.GetNeighbors( current.Item2) )
                {
                    var newCost = costSoFar[current.Item2] + graph.Cost( current.Item2, next );
                    if( !costSoFar.ContainsKey( next ) || newCost < costSoFar[next] )
                    {
                        costSoFar[next] = newCost;
                        var priority = newCost;
                        frontier.Add( new ValueTuple<int, T>(priority, next));
                        visitedNodes[next] = current.Item2;
                    }
                }

                frontier.Sort(comparer);
            }

            return null;
        }
    }
}
