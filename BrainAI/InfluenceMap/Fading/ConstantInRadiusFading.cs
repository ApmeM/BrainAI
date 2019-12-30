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
            var quadRadius = this.radius * this.radius;

            var vectorX = vector.X;
            var vectorY = vector.Y;

            var quadDist = vectorX * vectorX + vectorY * vectorY;

            if (quadDist > quadRadius)
            {
                return new Point();
            }

            var dist = Math.Sqrt(quadDist);
            var affectPower = chargeValue;

            return new Point(
                (int)(vectorX / dist * affectPower),
                (int)(vectorY / dist * affectPower));
        }
    }
}