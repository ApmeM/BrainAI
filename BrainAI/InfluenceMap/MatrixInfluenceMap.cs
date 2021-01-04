namespace BrainAI.InfluenceMap
{
    using BrainAI.InfluenceMap.Fading;
    using BrainAI.InfluenceMap.VectorGenerator;
    using BrainAI.Pathfinding;
    using System;

    public class MatrixInfluenceMap
    {
        public float[,] Map;

        public MatrixInfluenceMap(int width, int height)
        {
            this.Map = new float[width, height];
        }

        public void AddCharge(IChargeOrigin origin, IFading fading, float value)
        {
            var atPosition = new Point();
            for(var x = 0; x < Map.GetLength(0); x++)
                for(var y = 0; y < Map.GetLength(1); y++)
                {
                    atPosition.X = x;
                    atPosition.Y = y;
                    var vector = origin.GetVector(atPosition);
                    Map[atPosition.X, atPosition.Y] += fading.GetPower(Math.Abs(vector.X) + Math.Abs(vector.Y), value);
                }
        }
    }
}