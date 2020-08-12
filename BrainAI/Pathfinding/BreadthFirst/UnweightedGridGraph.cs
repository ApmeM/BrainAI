namespace BrainAI.Pathfinding.BreadthFirst
{
    using System.Collections.Generic;

    /// <summary>
    /// basic unweighted grid graph for use with the BreadthFirstPathfinder
    /// </summary>
    public class UnweightedGridGraph : IUnweightedGraph<Point>
    {
        private static readonly Point[] CardinalDirs = {
            new Point( 1, 0 ),
            new Point( 0, -1 ),
            new Point( -1, 0 ),
            new Point( 0, 1 ),
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

        public readonly int Width, Height;
        private readonly Point[] dirs;
        private readonly List<Point> neighbors = new List<Point>( 4 );


        public UnweightedGridGraph( int width, int height, bool allowDiagonalSearch = false )
        {
            this.Width = width;
            this.Height = height;
            this.dirs = allowDiagonalSearch ? CompassDirs : CardinalDirs;
        }

        public bool IsNodeInBounds( Point node )
        {
            return 0 <= node.X && node.X < this.Width && 0 <= node.Y && node.Y < this.Height;
        }


        public bool IsNodePassable( Point node )
        {
            return !this.Walls.Contains( node );
        }


        IEnumerable<Point> IUnweightedGraph<Point>.GetNeighbors( Point node )
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
    }
}