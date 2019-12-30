namespace BrainAI.Pathfinding.AStar
{
    using System;

    using BrainAI.Pathfinding.Dijkstra;

    /// <summary>
    /// basic static grid graph for use with A*. Add walls to the walls HashSet and weighted nodes to the weightedNodes HashSet. This provides
    /// a very simple grid graph for A* with just two weights: defaultWeight and weightedNodeWeight.
    /// </summary>
    public class AstarGridGraph : WeightedGridGraph, IAstarGraph<Point>
    {
        public AstarGridGraph( int width, int height, bool allowDiagonalSearch = false) : base(width, height, allowDiagonalSearch)
        {
        }

        public int Heuristic( Point node, Point goal )
        {
            return Math.Abs( node.X - goal.X ) + Math.Abs( node.Y - goal.Y );
        }
    }
}

