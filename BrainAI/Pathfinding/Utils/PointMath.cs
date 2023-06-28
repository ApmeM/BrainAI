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

        // In case points set in CCW order, the value will be positive, otherwice - negative.
        // Also method can be used as a dotprod between vectors p2-p1 and perpendicular to p0-p1
        public static double DoubledTriangleSquareBy3Dots(Point p0, Point p1, Point p2)
        {
            return (p0.Y - p1.Y) * (p2.X - p1.X) - (p0.X - p1.X) * (p2.Y - p1.Y);
        }

        public static double DotProdFor2VecotrsWithOneOrigin(Point p1, Point origin, Point p2)
        {
            return (p1.X - origin.X) * (p2.X - origin.X) + (p1.Y - origin.Y) * (p2.Y - origin.Y);
        }

        public static double PointToLineDistSq(Point center, Point p1, Point p2)
        {
            var triangle = PointMath.DoubledTriangleSquareBy3Dots(center, p1, p2);
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
                if (EqualityComparer<Point>.Default.Equals(lastPoint, p))
                {
                    return true;
                }

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
            var dotprod = PointMath.DotProdFor2VecotrsWithOneOrigin(center, p1, p2);
            if (dotprod <= 0.0 && PointMath.DistanceSquare(center, p1) > radiusSq)
            {
                return false;
            }

            dotprod = PointMath.DotProdFor2VecotrsWithOneOrigin(center, p2, p1);
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

            var lastPoint = points[points.Count - 1];

            var dotprodLast = PointMath.DoubledTriangleSquareBy3Dots(lastPoint, p1, p2);
            foreach (var currentPoint in points)
            {
                var dotprodCurrent = PointMath.DoubledTriangleSquareBy3Dots(currentPoint, p1, p2);
                var dotProdDirection = Math.Sign(dotprodLast) * Math.Sign(dotprodCurrent);
                if (dotProdDirection > 0 || finalDotsAreNotIntersections && dotProdDirection == 0)
                {
                    dotprodLast = dotprodCurrent;
                    lastPoint = currentPoint;
                    continue;
                }

                var dotprodP1 = PointMath.DoubledTriangleSquareBy3Dots(p1, currentPoint, lastPoint);
                var dotprodP2 = PointMath.DoubledTriangleSquareBy3Dots(p2, currentPoint, lastPoint);
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
            var basePoint = new Point(0, 0);
            for (int i = 0; i < points.Count; i++)
            {
                var p3 = points[i];
                var areaX2 = PointMath.DoubledTriangleSquareBy3Dots(p3, basePoint, p2);
                totalAreaX2 += areaX2;
                cx += (p2.X + p3.X) * areaX2;
                cy += (p2.Y + p3.Y) * areaX2;
                p2 = p3;
            }

            cx /= (3 * totalAreaX2);
            cy /= (3 * totalAreaX2);

            return (new Point((int)(cx + 0.5), (int)(cy + 0.5)), (totalAreaX2 > 0));
        }

        public static bool IsDirectionInsidePolygon(Point point, Point point2, List<Point> pointList, int pointIndex, double epsilon = 0.0001)
        {
            var pointPrevIndex = (pointIndex - 1 + pointList.Count) % pointList.Count;
            var pointNextIndex = (pointIndex + 1) % pointList.Count;

            var pointPrev = pointList[pointPrevIndex];
            var pointNext = pointList[pointNextIndex];

            var leftAngle = PointMath.DoubledTriangleSquareBy3Dots(pointPrev, point, point2);
            var rightAngle = PointMath.DoubledTriangleSquareBy3Dots(point2, point, pointNext);
            
            return Math.Sign(leftAngle) == Math.Sign(rightAngle) && Math.Sign(leftAngle) == 1;
        }
    }

}