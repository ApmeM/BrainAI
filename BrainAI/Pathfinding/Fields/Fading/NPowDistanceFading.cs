namespace BrainAI.Pathfinding.Fields.Fading
{
    using System;

    public class NPowDistanceFading : IFading
    {
        private readonly float pow;

        public NPowDistanceFading(float pow)
        {
            this.pow = pow;
        }

        public Point GetForce(Point chargePoint, float chargeValue, Point atPosition)
        {
            var vectorX = chargePoint.X - atPosition.X;
            var vectorY = chargePoint.Y - atPosition.Y;

            var quadDist = vectorX * vectorX + vectorY * vectorY;
            var dist = Math.Sqrt(quadDist);
            var affectPower = chargeValue / Math.Pow(quadDist, pow / 2);

            return new Point(
                (int)(vectorX / dist * affectPower),
                (int)(vectorY / dist * affectPower));
        }
    }
}