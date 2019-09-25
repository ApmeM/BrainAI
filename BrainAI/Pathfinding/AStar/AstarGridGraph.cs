namespace BrainAI.Pathfinding.AStar
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// basic static grid graph for use with A*. Add walls to the walls HashSet and weighted nodes to the weightedNodes HashSet. This provides
    /// a very simple grid graph for A* with just two weights: defaultWeight and weightedNodeWeight.
    /// </summary>
    public class AstarGridGraph : IAstarGraph<Point>
    {
        private static readonly Point[] CardinalDirs = {
            new Point( 1, 0 ),
            new Point( 0, -1 ),
            new Point( -1, 0 ),
            new Point( 0, 1 )
        };

        private static readonly Point[] CompassDirs = {
            new Point( 1, 0 ),
            new Point( 1, -1 ),
            new Point( 0, -1 ),
            new Point( -1, -1 ),
            new Point( -1, 0 ),
            new Point( -1, 1 ),
            new Point( 0, 1 ),
            new Point( 1, 1 ),
        };


        public HashSet<Point> Walls = new HashSet<Point>();
        public HashSet<Point> WeightedNodes = new HashSet<Point>();
        public int DefaultWeight = 1;
        public int WeightedNodeWeight = 5;

        private readonly int width, height;
        private readonly Point[] dirs;
        private readonly List<Point> neighbors = new List<Point>( 4 );

        public AstarGridGraph( int width, int height, bool allowDiagonalSearch = false)
        {
            this.width = width;
            this.height = height;
            this.dirs = allowDiagonalSearch ? CompassDirs : CardinalDirs;
        }

        /// <summary>
        /// ensures the node is in the bounds of the grid graph
        /// </summary>
        /// <returns><c>true</c>, if node in bounds was ised, <c>false</c> otherwise.</returns>
        /// <param name="node">Node.</param>
        bool IsNodeInBounds( Point node )
        {
            return 0 <= node.X && node.X < this.width && 0 <= node.Y && node.Y < this.height;
        }

        /// <summary>
        /// checks if the node is passable. Walls are impassable.
        /// </summary>
        /// <returns><c>true</c>, if node passable was ised, <c>false</c> otherwise.</returns>
        /// <param name="node">Node.</param>
        bool IsNodePassable( Point node )
        {
            return !this.Walls.Contains( node );
        }

        #region IAstarGraph implementation

        public IEnumerable<Point> GetNeighbors( Point node )
        {
            this.neighbors.Clear();

            foreach( var dir in this.dirs )
            {
                var next = new Point( node.X + dir.X, node.Y + dir.Y );
                if( this.IsNodeInBounds( next ) && this.IsNodePassable( next ) )
                    this.neighbors.Add( next );
            }

            return this.neighbors;
        }

        public int Cost( Point from, Point to )
        {
            return this.WeightedNodes.Contains( to ) ? this.WeightedNodeWeight : this.DefaultWeight;
        }

        public int Heuristic( Point node, Point goal )
        {
            return Math.Abs( node.X - goal.X ) + Math.Abs( node.Y - goal.Y );
        }

        #endregion

    }
}

