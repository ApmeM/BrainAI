namespace BrainAI.Pathfinding.Fields.Fading
{
    using System;

    public class LinearDistanceFading : IFading
    {
        private readonly double distanceEffectValue;

        public LinearDistanceFading(double distanceEffectValue)
        {
            this.distanceEffectValue = distanceEffectValue;
        }

        public Point GetForce(Point vector, float chargeValue)
        {
            var vectorX = vector.X;
            var vectorY = vector.Y;

            var quadDist = vectorX * vectorX + vectorY * vectorY;
            var dist = Math.Sqrt(quadDist);
            double affectPower;
            if (chargeValue > 0)
            {
                affectPower = chargeValue - Math.Min(chargeValue, dist * this.distanceEffectValue);
            }
            else
            {
                affectPower = chargeValue + Math.Min(-chargeValue, dist * this.distanceEffectValue);
            }

            return new Point(
                (int)(vectorX / dist * affectPower),
                (int)(vectorY / dist * affectPower));
        }
    }
}