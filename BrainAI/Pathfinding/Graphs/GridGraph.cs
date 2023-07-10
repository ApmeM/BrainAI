namespace BrainAI.Pathfinding
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Basic grid graph for use with all pathfinding algorithms
    /// </summary>
    public class GridGraph : IAstarGraph<Point>
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
        public Dictionary<Point, int> Weights = new Dictionary<Point, int>();
        public int DefaultWeight = 1;

        public readonly int Width, Height;

        private readonly Point[] dirs;
        private readonly List<Point> neighbors = new List<Point>(4);

        public GridGraph(int width, int height, bool allowDiagonalSearch = false)
        {
            this.Width = width;
            this.Height = height;
            this.dirs = allowDiagonalSearch ? CompassDirs : CardinalDirs;
        }

        public List<Point> GetNeighbors(Point node)
        {
            this.neighbors.Clear();

            foreach (var dir in this.dirs)
            {
                var next = new Point(node.X + dir.X, node.Y + dir.Y);
                if (this.IsNodeInBounds(next) && this.IsNodePassable(next))
                    this.neighbors.Add(next);
            }

            return this.neighbors;
        }

        public int Cost(Point from, Point to)
        {
            return this.Weights.ContainsKey(to) ? this.Weights[to] : this.DefaultWeight;
        }

        public int Heuristic(Point node, Point goal)
        {
            return Math.Abs(node.X - goal.X) + Math.Abs(node.Y - goal.Y);
        }

        private bool IsNodeInBounds(Point node)
        {
            return 0 <= node.X && node.X < this.Width && 0 <= node.Y && node.Y < this.Height;
        }

        private bool IsNodePassable(Point node)
        {
            return !this.Walls.Contains(node);
        }

        public void BeforeSearch(Point nodeStart, HashSet<Point> nodeEnd)
        {
        }

        private StringBuilder sb = new StringBuilder();

        public override string ToString()
        {
            sb.Clear();

            for (var y = 0; y < this.Height; y++)
            {
                for (var x = 0; x < this.Width; x++)
                {
                    var pos = new Point(x, y);
                    var isWall = this.Walls.Contains(pos);
                    if (isWall)
                    {
                        sb.Append("#");
                    }
                    else
                    {
                        sb.Append(".");
                    }
                }

                sb.Append("\n");
            }

            return sb.ToString();
        }
    }
}