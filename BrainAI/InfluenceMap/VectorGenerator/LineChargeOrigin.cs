namespace BrainAI.InfluenceMap.VectorGenerator
{
    using System;

    using BrainAI.Pathfinding;

    public class LineChargeOrigin : IChargeOrigin
    {

        public LineChargeOrigin(Point p1, Point p2)
        {
            this.P1 = p1;
            this.P2 = p2;
            this.V = new Point(p2.X - p1.X, p2.Y - p1.Y);
            this.VP = new Point(this.V.Y, -this.V.X);
        }

        public Point P1 { get; set; }
        public Point P2 { get; set; }

        private readonly Point V;
        private readonly Point VP;

        public Point GetVector(Point toPoint)
        {
            var x1 = this.P1.X;
            var y1 = this.P1.Y;
            var x2 = this.P2.X;
            var y2 = this.P2.Y;
            var x3 = toPoint.X;
            var y3 = toPoint.Y;
            var x4 = toPoint.X + this.VP.X;
            var y4 = toPoint.Y + this.VP.Y;

            var length = ((x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4));
            float x, y;
            if (Math.Abs(length) < 0.000001)
            {
                x = this.P1.X;
                y = this.P1.Y;
            }
            else
            {
                x = ((x1 * y2 - y1 * x2) * (x3 - x4) - (x1 - x2) * (x3 * y4 - y3 * x4)) / length;
                y = ((x1 * y2 - y1 * x2) * (y3 - y4) - (y1 - y2) * (x3 * y4 - y3 * x4)) / length;
            }

            return new Point(toPoint.X - (int)x, toPoint.Y - (int)y);
        }

        public override string ToString()
        {
            return $"Line ({this.P1} - {this.P2})";
        }
    }
}