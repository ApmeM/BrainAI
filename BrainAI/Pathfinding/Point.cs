using System;

namespace BrainAI.Pathfinding
{
    public struct Point : IEquatable<Point>
    {
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; set; }
        public int Y { get; set; }

        public override string ToString()
        {
            return $"({X} x {Y})";
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return (obj is Point other) && this.Equals(other);
        }

        public bool Equals(Point obj)
        {
            return this.X == obj.X && this.Y == obj.Y;
        }

        public static bool operator ==(Point a, Point b) => a.X == b.X && a.Y == b.Y;

        public static bool operator !=(Point a, Point b) => a.X != b.X || a.Y != b.Y;
    }
}