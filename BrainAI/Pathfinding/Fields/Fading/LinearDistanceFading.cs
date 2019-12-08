namespace BrainAI.Pathfinding.Fields.Fading
{
    using System;

    public class LinearDistanceFading : IFading
    {
        public Point GetForce(Point vector, float chargeValue)
        {
            var vectorX = vector.X;
            var vectorY = vector.Y;

            var quadDist = vectorX * vectorX + vectorY * vectorY;
            var dist = Math.Sqrt(quadDist);
            double affectPower;
            if (chargeValue > 0)
            {
                affectPower = chargeValue - Math.Min(chargeValue, dist);
            }
            else
            {
                affectPower = chargeValue + Math.Min(-chargeValue, dist);
            }

            return new Point(
                (int)(vectorX / dist * affectPower),
                (int)(vectorY / dist * affectPower));
        }
    }
}