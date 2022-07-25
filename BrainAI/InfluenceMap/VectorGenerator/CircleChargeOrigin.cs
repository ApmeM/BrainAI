namespace BrainAI.InfluenceMap.VectorGenerator
{
    using System;
    using BrainAI.Pathfinding;

    public class CircleChargeOrigin : IChargeOrigin
    {

        public CircleChargeOrigin(Point fromPoint, int radius)
        {
            this.FromPoint = fromPoint;
            this.Radius = radius;
        }

        public int Radius { get; set; }
        public Point FromPoint { get; set; }

        public Point GetVector(Point toPoint)
        {
            var vectorToCircleX = toPoint.X - this.FromPoint.X;
            var vectorToCircleY = toPoint.Y - this.FromPoint.Y;
            var vectorToCircleLength = Math.Sqrt(vectorToCircleX * vectorToCircleX + vectorToCircleY * vectorToCircleY);

            var pointOnCircleX = vectorToCircleX * Radius / vectorToCircleLength + FromPoint.X;
            var pointOnCircleY = vectorToCircleY * Radius / vectorToCircleLength + FromPoint.Y;

            var vectorX = toPoint.X - pointOnCircleX;
            var vectorY = toPoint.Y - pointOnCircleY;
            return new Point((int)vectorX, (int)vectorY);
        }

        public override string ToString()
        {
            return $"Point {this.FromPoint}";
        }
    }
}