namespace BrainAI.Pathfinding.Fields
{
    using System.Collections.Generic;

    using BrainAI.Pathfinding.Fields.Fading;

    public class InfluenceMap
    {
        public static readonly IFading QuadDistanceFading = new QuadDistanceFading();
        public static readonly IFading QuadQuadDistanceFading = new TripleDistanceFading();
        public static readonly IFading LinearDistanceFading = new QuadDistanceFading();
        public static readonly IFading ConstantFading = new NoDistanceFading();

        public readonly List<Charge> Charges = new List<Charge>();

        public Point FindForceDirection(Point atPosition)
        {
            var result = new Point();
            foreach (var charge in this.Charges)
            {
                var force = charge.Fading.GetForce(charge.Point, charge.Value, atPosition);
                result.X += force.X;
                result.Y += force.Y;
            }
            return result;
        }

        public class Charge
        {
            public Point Point;
            public float Value;
            public IFading Fading;
        }
    }
}