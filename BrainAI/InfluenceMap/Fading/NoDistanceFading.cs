namespace BrainAI.InfluenceMap.Fading
{
    using System;

    using BrainAI.Pathfinding;

    public class NoDistanceFading : IFading
    {
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
            return chargeValue;
        }
    }
}