namespace BrainAI.InfluenceMap.Fading
{
    using System;

    using BrainAI.Pathfinding;

    public class ConstantInRadiusFading : IFading
    {
        private readonly float radius;

        public ConstantInRadiusFading(float radius)
        {
            this.radius = radius;
        }

        public Point GetForce(Point vector, float chargeValue)
        {
            var vectorX = vector.X;
            var vectorY = vector.Y;

            var quadDist = vectorX * vectorX + vectorY * vectorY;
            var dist = (float)Math.Sqrt(quadDist);
            var affectPower = GetPower(dist, chargeValue);

            return new Point(
                (int)(vector.X / dist * affectPower),
                (int)(vector.Y / dist * affectPower));
        }

        public float GetPower(float distance, float chargeValue)
        {
            return distance > radius ? 0 : chargeValue;
        }
    }
}