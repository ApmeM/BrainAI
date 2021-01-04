namespace BrainAI.InfluenceMap.Fading
{
    using System;

    using BrainAI.Pathfinding;

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
            var dist = (float)Math.Sqrt(quadDist);
            var affectPower = GetPower(dist, chargeValue);

            return new Point(
                (int)(vectorX / dist * affectPower),
                (int)(vectorY / dist * affectPower));
        }

        public float GetPower(float distance, float chargeValue)
        {
            float affectPower;
            if (chargeValue > 0)
            {
                affectPower = chargeValue - (float)Math.Min(chargeValue, distance * this.distanceEffectValue);
            }
            else
            {
                affectPower = chargeValue + (float)Math.Min(-chargeValue, distance * this.distanceEffectValue);
            }

            return affectPower;
        }
    }
}