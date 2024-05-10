using System;

namespace BrainAI.Pathfinding
{
    public struct Point : IEquatable<Point>, IComparable<Point>
    {
        public static Point Zero = new Point(0, 0);
        public static Point Left = new Point(-1, 0);
        public static Point Right = new Point(1, 0);
        public static Point Up = new Point(0, -1);
        public static Point Down = new Point(0, 1);

        public int X;
        public int Y;

        public Point(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public int ManhattanLength => Math.Abs(this.X) + Math.Abs(this.Y);
        public int Length => (int)Math.Sqrt(this.LengthQuad);

        public int LengthQuad => this.X * this.X + this.Y * this.Y;

        public Point Normalize() => this / this.Length;

        public int Angle => (int)Math.Atan2(this.Y, this.X);

        public static Point operator +(Point a, Point b)
            => new Point(a.X + b.X, a.Y + b.Y);

        public static Point operator -(Point a, Point b)
                    => new Point(a.X - b.X, a.Y - b.Y);

        public static Point operator -(Point a)
                    => new Point(-a.X, -a.Y);

        public static Point operator *(Point a, int b)
                    => new Point(a.X * b, a.Y * b);

        public static Point operator *(int b, Point a)
                    => new Point(a.X * b, a.Y * b);

        public static Point operator /(Point a, int b)
                    => new Point(a.X / b, a.Y / b);

        public static Point operator /(int b, Point a)
                    => new Point(a.X / b, a.Y / b);

        public static bool operator ==(Point a, Point b)
                    => a.X == b.X && a.Y == b.Y;

        public static bool operator !=(Point a, Point b)
                    => !(a == b);

        public int Dot(Point p2)
        {
            return this.X * p2.X + this.Y * p2.Y;
        }

        // In case points set in CCW order, the value will be positive, otherwice - negative.
        // Method can be used as a dotprod between vectors this and perpendicular to p2
        // The overal result value is equl to doubled triangle size between those 2 points and point (0,0)
        public int Cross(Point p2)
        {
            return this.X * p2.Y - this.Y * p2.X;
        }

        public Point Rotate(int angle)
        {
            var cos = (int)Math.Cos(angle);
            var sin = (int)Math.Sin(angle);

            return new Point(cos * this.X - sin * this.Y, sin * this.X + cos * this.Y);
        }

        public int AngleToVector(Point vec2)
        {
            return (int)Math.Acos((this.X * vec2.X + this.Y * vec2.Y) / (this.Length * vec2.Length));
        }

        public override bool Equals(object obj)
        {
            return obj is Point vector && this.Equals(vector);
        }

        public override int GetHashCode()
        {
            int hashCode = -1274299002;
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            return hashCode;
        }

        public bool Equals(Point vector)
        {
            return X == vector.X &&
                   Y == vector.Y;
        }

        /// Comparison is done by angle in CCW order from 0 to 2PI. 
        public int CompareTo(Point other)
        {
            if (this.Y == 0 && other.Y == 0)
            {
                return Math.Sign(Math.Sign(this.X) - Math.Sign(other.X));
            }

            if ((this.Y >= 0) ^ (other.Y >= 0))
            {
                if (this.Y >= 0)
                    return 1;
                else
                    return -1;
            }
            else
            {
                return Math.Sign((other - this).Cross(-this));
            }
        }

        public override string ToString(){
            return $"({X}, {Y})";
        }
    }
}
