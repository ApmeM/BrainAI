namespace BrainAI.Pathfinding.Dijkstra
{
    using System.Collections.Generic;

    using BrainAI.Pathfinding.BreadthFirst;

    /// <summary>
    /// basic grid graph with support for one type of weighted node
    /// </summary>
    public class WeightedGridGraph : UnweightedGridGraph, IWeightedGraph<Point>
    {
        public HashSet<Point> WeightedNodes = new HashSet<Point>();
        public int DefaultWeight = 1;
        public int WeightedNodeWeight = 5;


        public WeightedGridGraph( int width, int height, bool allowDiagonalSearch = false ) : base(width, height, allowDiagonalSearch)
        {
        }

        public int Cost( Point from, Point to )
        {
            return this.WeightedNodes.Contains( to ) ? this.WeightedNodeWeight : this.DefaultWeight;
        }
    }
}

