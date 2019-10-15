namespace BrainAI.Pathfinding.Fields.Fading
{
    using System;

    public class LinearDistanceFading : IFading
    {
        public Point GetForce(Point chargePoint, float chargeValue, Point atPosition)
        {
            var vectorX = chargePoint.X - atPosition.X;
            var vectorY = chargePoint.Y - atPosition.Y;

            var quadDist = vectorX * vectorX + vectorY * vectorY;
            var dist = Math.Sqrt(quadDist);
            var affectPower = chargeValue / dist;

            return new Point(
                (int)(vectorX / dist * affectPower),
                (int)(vectorY / dist * affectPower));
        }
    }
}