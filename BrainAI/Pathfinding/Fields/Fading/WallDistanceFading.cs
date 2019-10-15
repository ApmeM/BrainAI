namespace BrainAI.Pathfinding.Fields.Fading
{
    using System;

    public class WallDistanceFading : IFading
    {
        private readonly float perpendicularVectorX;

        private readonly float perpendicularVectorY;

        public WallDistanceFading(float perpendicularVectorX, float perpendicularVectorY)
        {
            this.perpendicularVectorX = perpendicularVectorX;
            this.perpendicularVectorY = perpendicularVectorY;
        }

        public Point GetForce(Point chargePoint, float chargeValue, Point atPosition)
        {
            var vx = this.perpendicularVectorX;
            var vy = this.perpendicularVectorY;
            var cpx = chargePoint.X;
            var cpy = chargePoint.Y;
            var px = atPosition.X;
            var py = atPosition.Y;

            var vpx = vy;
            var vpy = -vx;

            var x1 = cpx;
            var y1 = cpy;
            var x2 = cpx + vpx;
            var y2 = cpy + vpy;
            var x3 = px;
            var y3 = py;
            var x4 = px + vx;
            var y4 = py + vy;

            var length = ((x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4));
            float x, y;
            if (Math.Abs(length) < 0.000001)
            {
                x = cpx;
                y = cpy;
            }
            else
            {
                x = ((x1 * y2 - y1 * x2) * (x3 - x4) - (x1 - x2) * (x3 * y4 - y3 * x4)) / length;
                y = ((x1 * y2 - y1 * x2) * (y3 - y4) - (y1 - y2) * (x3 * y4 - y3 * x4)) / length;
            }

            var vectorX1 = x - atPosition.X;
            var vectorY1 = y - atPosition.Y;

            var quadDist = vectorX1 * vectorX1 + vectorY1 * vectorY1;
            var dist = Math.Sqrt(quadDist);
            var affectPower = chargeValue / quadDist;

            return new Point(
                (int)(vectorX1 / dist * affectPower),
                (int)(vectorY1 / dist * affectPower));
        }
    }
}