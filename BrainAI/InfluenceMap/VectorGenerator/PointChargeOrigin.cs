namespace BrainAI.InfluenceMap.VectorGenerator
{
    using BrainAI.Pathfinding;

    public class PointChargeOrigin : IChargeOrigin
    {
        public PointChargeOrigin(Point fromPoint)
        {
            this.FromPoint = fromPoint;
        }

        public Point FromPoint { get; set; }

        public Point GetVector(Point toPoint)
        {
            var vectorX = toPoint.X - this.FromPoint.X;
            var vectorY = toPoint.Y - this.FromPoint.Y;
            return new Point(vectorX, vectorY);
        }

        public override string ToString()
        {
            return $"Point {this.FromPoint}";
        }
    }
}