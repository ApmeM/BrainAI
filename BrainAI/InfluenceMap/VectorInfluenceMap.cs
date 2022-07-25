namespace BrainAI.InfluenceMap
{
    using System.Collections.Generic;

    using BrainAI.InfluenceMap.Fading;
    using BrainAI.InfluenceMap.VectorGenerator;
    using BrainAI.Pathfinding;

    public class VectorInfluenceMap
    {
        private readonly Dictionary<string, List<Charge>> Map = new Dictionary<string, List<Charge>>();

        public void AddCharge(string layerName, IChargeOrigin origin, IFading fading, float value)
        {
            if (!Map.ContainsKey(layerName))
            {
                this.Map[layerName] = new List<Charge>();
            }

            var charges = this.Map[layerName];
            charges.Add(new Charge
            {
                Fading = fading,
                Origin = origin,
                Value = value
            });
        }

        public void ClearLayer(string layerName)
        {
            if (!Map.ContainsKey(layerName))
            {
                return;
            }
            
            Map.Remove(layerName);
        }

        public Point FindForceDirection(Point atPosition)
        {
            var result = new Point();
            foreach (var layer in this.Map)
            foreach (var charge in layer.Value)
            {
                var vector = charge.Origin.GetVector(atPosition);
                var force = charge.Fading.GetForce(vector, charge.Value);
                result.X -= force.X;
                result.Y -= force.Y;
            }
            return result;
        }

        private class Charge
        {
            public float Value;
            public IChargeOrigin Origin;
            public IFading Fading;

            public override string ToString()
            {
                return $"Charge at {this.Origin} with {this.Value}";
            }
        }
    }
}