namespace BrainAI.InfluenceMap
{
    using System.Collections.Generic;

    using BrainAI.InfluenceMap.Fading;
    using BrainAI.InfluenceMap.VectorGenerator;
    using BrainAI.Pathfinding;

    public class VectorInfluenceMap
    {
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