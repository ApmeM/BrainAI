namespace BrainAI.Pathfinding.Fields.Fading
{
    public class PointChargeOrigin : IChargeOrigin
    {
        public PointChargeOrigin(Point fromPoint)
        {
            FromPoint = fromPoint;
        }

        public Point FromPoint { get; set; }

        public Point GetVector(Point toPoint)
        {
            var vectorX = toPoint.X - FromPoint.X;
            var vectorY = toPoint.Y - FromPoint.Y;
            return new Point(vectorX, vectorY);
        }

        public override string ToString()
        {
            return $"Point {FromPoint}";
        }
    }
}