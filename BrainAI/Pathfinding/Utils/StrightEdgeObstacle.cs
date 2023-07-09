using System;
using System.Collections.Generic;

namespace BrainAI.Pathfinding
{
    internal struct StrightEdgeObstacle
    {
        public List<Point> points;
        public Point center;
        public double radiusSq;

        public StrightEdgeObstacle(List<Point> pointsList)
        {
            if (pointsList.Count < 3)
            {
                throw new Exception($"Minimum of 3 points needed. pointsList.Count == {pointsList.Count}");
            }

            var (centerPoint, ccw) = PointMath.CalcCenterOfPolygon(pointsList);

            this.points = pointsList;
            this.center = centerPoint;
            this.radiusSq = PointMath.CalcRadiusSquare(pointsList, centerPoint);

            if (!ccw)
            {
                this.points.Reverse();
            }
        }

        public bool Equals(StrightEdgeObstacle obj)
        {
            return this.center == obj.center && this.radiusSq == obj.radiusSq && this.points == obj.points;
        }
    }
}