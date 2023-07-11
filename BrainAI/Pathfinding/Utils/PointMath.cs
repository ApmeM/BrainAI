using System;
using System.Collections.Generic;

namespace BrainAI.Pathfinding
{
    public static class PointMath
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

        public static double CalcRadiusSquare(Lookup<int, Point>.Enumerable points, Point center)
        {
            var radiusSq = -1d;
            foreach (var point in points)
            {
                var currentRadiusSq = PointMath.DistanceSquare(point, center);
                if (currentRadiusSq > radiusSq)
                {
                    radiusSq = currentRadiusSq;
                }
            }

            return radiusSq;
        }

        public static bool PointWithinRectangle(Point minPoint, Point maxPoint, Point p)
        {
            return minPoint.X <= p.X && p.X <= maxPoint.X && minPoint.Y <= p.Y && p.Y <= maxPoint.Y;
        }

        public static bool PointWithinPolygon(Lookup<int, Point>.Enumerable points, Point p)
        {
            Point? lastPoint = null;
            var crossings = 0;
            foreach (var currentPoint in new ExtendedEnumerable<Point>(points, points.Count + 1))
            {
                try
                {
                    if (lastPoint == null)
                    {
                        continue;
                    }

                    if (lastPoint == p)
                    {
                        return true;
                    }

                    if (((lastPoint.Value.Y <= p.Y && p.Y < currentPoint.Y) || (currentPoint.Y <= p.Y && p.Y < lastPoint.Value.Y))
                            && p.X < ((currentPoint.X - lastPoint.Value.X) / (currentPoint.Y - lastPoint.Value.Y) * (p.Y - lastPoint.Value.Y) + lastPoint.Value.X))
                    {
                        crossings++;
                    }
                }
                finally
                {
                    lastPoint = currentPoint;
                }
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

        public static bool SegmentIntersectsPolygon(Lookup<int, Point>.Enumerable points, Point p1, Point p2, bool finalDotsAreNotIntersections)
        {
            if (p1 == p2)
            {
                return false;
            }

            Point? lastPoint = null;
            var dotprodLast = 0d;
            foreach (var currentPoint in new ExtendedEnumerable<Point>(points, points.Count + 1))
            {
                var dotprodCurrent = PointMath.DoubledTriangleSquareBy3Dots(currentPoint, p1, p2);

                try
                {
                    if (lastPoint == null)
                    {
                        continue;
                    }

                    if (finalDotsAreNotIntersections && (p1 == currentPoint || p2 == currentPoint || p1 == lastPoint || p2 == lastPoint))
                    {
                        continue;
                    }

                    if (Math.Sign(dotprodLast) == Math.Sign(dotprodCurrent))
                    {
                        continue;
                    }

                    var dotprodP1 = PointMath.DoubledTriangleSquareBy3Dots(p1, currentPoint, lastPoint.Value);
                    var dotprodP2 = PointMath.DoubledTriangleSquareBy3Dots(p2, currentPoint, lastPoint.Value);
                    if (Math.Sign(dotprodP1) == Math.Sign(dotprodP2))
                    {
                        continue;
                    }

                    return true;
                }
                finally
                {
                    dotprodLast = dotprodCurrent;
                    lastPoint = currentPoint;
                }
            }

            return false;
        }

        public static bool IsDirectionInsidePolygon(Point point, Point point2, Point pointPrev, Point pointNext, double epsilon = 0.0001)
        {
            var leftAngle = PointMath.DoubledTriangleSquareBy3Dots(pointPrev, point, point2);
            var rightAngle = PointMath.DoubledTriangleSquareBy3Dots(point2, point, pointNext);

            return Math.Sign(leftAngle) == Math.Sign(rightAngle) && Math.Sign(leftAngle) == 1;
        }
    }

}