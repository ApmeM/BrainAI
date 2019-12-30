namespace BrainAI.InfluenceMap
{
    using System.Collections.Generic;

    using BrainAI.InfluenceMap.Fading;
    using BrainAI.InfluenceMap.VectorGenerator;
    using BrainAI.Pathfinding;

    public class InfluenceMap
    {
        public static readonly IFading NoDistanceFading = new NoDistanceFading();
        public static readonly IFading LinearDistanceFading = new LinearDistanceFading(1);
        public static readonly IFading DistanceFading = new NPowDistanceFading(1);
        public static readonly IFading QuadDistanceFading = new NPowDistanceFading(2);
        public static readonly IFading TripleDistanceFading = new NPowDistanceFading(3);

        public readonly List<Charge> Charges = new List<Charge>();

        public Point FindForceDirection(Point atPosition)
        {
            var result = new Point();
            foreach (var charge in this.Charges)
            {
                var vector = charge.Origin.GetVector(atPosition);
                var force = charge.Fading.GetForce(vector, charge.Value);
                result.X -= force.X;
                result.Y -= force.Y;
            }
            return result;
        }

        public class Charge
        {
            public string Name;
            public float Value;
            public IChargeOrigin Origin;
            public IFading Fading;

            public override string ToString()
            {
                return $"{this.Name ?? "Charge"} at {this.Origin} with {this.Value}";
            }
        }
    }
}