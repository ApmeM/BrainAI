namespace BrainAI.InfluenceMap
{
    using BrainAI.InfluenceMap.Fading;
    using BrainAI.InfluenceMap.VectorGenerator;
    using BrainAI.Pathfinding;
    using System;
    using System.Collections.Generic;

    public class MatrixInfluenceMap
    {
        private readonly int width;
        private readonly int height;
        private Dictionary<string, float[,]> Map = new Dictionary<string, float[,]>();
        private float[,] totalMap;
        private bool totalMapDirty = false;

        public MatrixInfluenceMap(int width, int height)
        {
            this.width = width;
            this.height = height;
            this.totalMap = new float[width, height];
        }

        public void AddCharge(string layerName, IChargeOrigin origin, IFading fading, float value)
        {
            var atPosition = new Point();
            if (!Map.ContainsKey(layerName))
            {
                Map[layerName] = new float[width, height];
            }

            var map = Map[layerName];
            for (var x = 0; x < map.GetLength(0); x++)
                for (var y = 0; y < map.GetLength(1); y++)
                {
                    atPosition.X = x;
                    atPosition.Y = y;
                    var vector = origin.GetVector(atPosition);
                    map[atPosition.X, atPosition.Y] += fading.GetPower(Math.Abs(vector.X) + Math.Abs(vector.Y), value);
                    totalMap[atPosition.X, atPosition.Y] += fading.GetPower(Math.Abs(vector.X) + Math.Abs(vector.Y), value);
                }
        }

        public void ClearLayer(string layerName)
        {
            if (!Map.ContainsKey(layerName))
            {
                return;
            }

            Map.Remove(layerName);
            totalMapDirty = true;
        }

        public Point FindForceDirection(Point atPosition)
        {
            var min = float.MaxValue;
            var direction = atPosition;
            for (var x = -1; x < 2; x++)
                for (var y = -1; y < 2; y++)
                {
                    var currentCharge = GetChargeAtPoint(new Point(atPosition.X + x, atPosition.Y + y));
                    if (currentCharge < min)
                    {
                        min = currentCharge;
                        direction = new Point(atPosition.X + x, atPosition.Y + y);
                    }
                }

            return direction;
        }

        public float GetChargeAtPoint(Point atPosition)
        {
            if (!totalMapDirty)
            {
                return totalMap[atPosition.X, atPosition.Y];
            }

            var currentCharge = 0f;

            foreach (var item in Map)
            {
                currentCharge += item.Value[atPosition.X, atPosition.Y];
            }

            return currentCharge;
        }

        public void CalculateTotalMap()
        {
            if (!totalMapDirty)
            {
                return;
            }
            for (var x = 0; x < totalMap.GetLength(0); x++)
                for (var y = 0; y < totalMap.GetLength(1); y++)
                {
                    totalMap[x, y] = GetChargeAtPoint(new Point(x, y));
                }
            totalMapDirty = false;
        }
    }
}