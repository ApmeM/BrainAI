using System;
using System.Collections.Generic;

namespace BrainAI.Pathfinding
{
    internal static class PointMath
    {
        public static double DistanceSquare(Point p2, Point p)
        {
            return (p2.X - p.X) * (p2.X - p.X) + (p2.Y - p.Y) * (p2.Y - p.Y);
        }

        public static double RelCCWDouble(Point center, Point pointBefore, Point pointAfter)
        {
            return (center.Y - pointBefore.Y) * (pointAfter.X - pointBefore.X) - (center.X - pointBefore.X) * (pointAfter.Y - pointBefore.Y);
        }

        public static double PointToLineDistSq(Point center, Point p1, Point p2)
        {
            var triangle = (p2.X - p1.X) * (center.Y - p1.Y) - (center.X - p1.X) * (p2.Y - p1.Y);
            return triangle * triangle / PointMath.DistanceSquare(p2, p1);
        }

        public static double CalcRadiusSquare(List<Point> points, Point center)
        {
            var radiusSq = -1d;
            for (int i = 0; i < points.Count; i++)
            {
                var currentRadiusSq = PointMath.DistanceSquare(points[i], center);
                if (currentRadiusSq > radiusSq)
                {
                    radiusSq = currentRadiusSq;
                }
            }

            return radiusSq;
        }

        public static bool PointWithinPolygon(List<Point> points, Point p)
        {
            var lastPoint = points[points.Count - 1];
            var crossings = 0;
            foreach (var currentPoint in points)
            {
                if (((lastPoint.Y <= p.Y && p.Y < currentPoint.Y) || (currentPoint.Y <= p.Y && p.Y < lastPoint.Y))
                        && p.X < ((currentPoint.X - lastPoint.X) / (currentPoint.Y - lastPoint.Y) * (p.Y - lastPoint.Y) + lastPoint.X))
                {
                    crossings++;
                }
                lastPoint = currentPoint;
            }

            return (crossings % 2 != 0);
        }


        public static bool SegmentIntersectCircle(Point p1, Point p2, Point center, double radiusSq)
        {
            var dotprod = (center.X - p1.X) * (p2.X - p1.X) + (center.Y - p1.Y) * (p2.Y - p1.Y);
            if (dotprod <= 0.0 && PointMath.DistanceSquare(center, p1) > radiusSq)
            {
                return false;
            }

            dotprod = (center.X - p2.X) * (p1.X - p2.X) + (center.Y - p2.Y) * (p1.Y - p2.Y);
            if (dotprod <= 0.0 && PointMath.DistanceSquare(center, p2) > radiusSq)
            {
                return false;
            }

            if (PointMath.PointToLineDistSq(center, p2, p1) > radiusSq)
            {
                return false;
            }

            return true;
        }

        public static bool SegmentIntersectsPolygon(List<Point> points, Point p1, Point p2, bool finalDotsAreNotIntersections)
        {
            if (p1.Equals(p2))
            {
                return false;
            }

            // Perpendicular vector to segment:
            var vx = p1.Y - p2.Y;
            var vy = p2.X - p1.X;

            var lastPoint = points[points.Count - 1];
            var dotprodLast = (lastPoint.X - p1.X) * vx + (lastPoint.Y - p1.Y) * vy;
            foreach (var currentPoint in points)
            {
                var dotprodCurrent = (currentPoint.X - p1.X) * vx + (currentPoint.Y - p1.Y) * vy;
                var dotProdDirection = Math.Sign(dotprodLast) * Math.Sign(dotprodCurrent);
                if (dotProdDirection > 0 || finalDotsAreNotIntersections && dotProdDirection == 0)
                {
                    dotprodLast = dotprodCurrent;
                    lastPoint = currentPoint;
                    continue;
                }

                var v2x = currentPoint.Y - lastPoint.Y;
                var v2y = lastPoint.X - currentPoint.X;

                var dotprodP1 = (p1.X - currentPoint.X) * v2x + (p1.Y - currentPoint.Y) * v2y;
                var dotprodP2 = (p2.X - currentPoint.X) * v2x + (p2.Y - currentPoint.Y) * v2y;
                dotProdDirection = Math.Sign(dotprodP1) * Math.Sign(dotprodP2);
                if (dotProdDirection > 0 || finalDotsAreNotIntersections && dotProdDirection == 0)
                {
                    dotprodLast = dotprodCurrent;
                    lastPoint = currentPoint;
                    continue;
                }

                return true;
            }

            return false;
        }

        // Result:
        // Point - center point of the polygon
        // bool - is polygon counter-clockwise or not
        public static (Point, bool) CalcCenterOfPolygon(List<Point> points)
        {
            if (points.Count == 1)
            {
                return (points[0], true);
            }
            if (points.Count == 2)
            {
                return (new Point((int)((points[0].X + points[1].X) / 2f), (int)((points[0].X + points[1].X) / 2f)), true);
            }

            var cx = 0.0;
            var cy = 0.0;
            var p2 = points[points.Count - 1];
            var totalAreaX2 = 0d;
            for (int i = 0; i < points.Count; i++)
            {
                var p3 = points[i];
                var areaX2 = ((p2.X * p3.Y) - (p2.Y * p3.X));
                totalAreaX2 += areaX2;
                cx += (p2.X + p3.X) * areaX2;
                cy += (p2.Y + p3.Y) * areaX2;
                p2 = p3;
            }

            cx /= (3 * totalAreaX2);
            cy /= (3 * totalAreaX2);

            return (new Point((int)(cx + 0.5), (int)(cy + 0.5)), (totalAreaX2 > 0));
        }
    }
}