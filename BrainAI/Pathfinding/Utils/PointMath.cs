using System;

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

        public static bool IsDirectionInsidePolygon(Point point, Point point2, Point pointPrev, Point pointNext, bool concave)
        {
            var leftAngle = PointMath.DoubledTriangleSquareBy3Dots(pointPrev, point, point2);
            var rightAngle = PointMath.DoubledTriangleSquareBy3Dots(point2, point, pointNext);

            return !concave && Math.Sign(leftAngle) == Math.Sign(rightAngle) && Math.Sign(leftAngle) == 1 ||
                    concave && !(Math.Sign(leftAngle) == Math.Sign(rightAngle) && Math.Sign(leftAngle) == -1);
        }

        public static bool SegmentIntersectsSegment(Point p11, Point p12, Point p21, Point p22)
        {
            return p21 == p12 || p22 == p12 || p21 == p11 || p22 == p11 ||
                Math.Sign(PointMath.DoubledTriangleSquareBy3Dots((Point)p12, (Point)p21, (Point)p22)) != Math.Sign(PointMath.DoubledTriangleSquareBy3Dots((Point)p11, (Point)p21, (Point)p22)) &&
                Math.Sign(PointMath.DoubledTriangleSquareBy3Dots((Point)p21, (Point)p12, (Point)p11)) != Math.Sign(PointMath.DoubledTriangleSquareBy3Dots((Point)p22, (Point)p12, (Point)p11));
        }

        public static int CompareVectors(Point first, Point origin, Point second)
        {
            var v1 = new Point(first.X - origin.X, first.Y - origin.Y);
            var v2 = new Point(second.X - origin.X, second.Y - origin.Y);

            if (v1.Y == 0 && v2.Y == 0)
            {
                return Math.Sign(Math.Sign(v1.X) - Math.Sign(v2.X));
            }

            if ((v1.Y >= 0) ^ (v2.Y >= 0))
            {
                if (v1.Y >= 0)
                    return 1;
                else
                    return -1;
            }
            else
            {
                return Math.Sign(PointMath.DoubledTriangleSquareBy3Dots(new Point(0, 0), v1, v2));
            }
        }

        public static (int, Point)? FindEndPoint(Point center, (int, Point) startPoint, Point p1, Point p2)
        {
            var dir1 = PointMath.DoubledTriangleSquareBy3Dots(center, startPoint.Item2, p1) < 0;
            var dir2 = PointMath.DoubledTriangleSquareBy3Dots(center, startPoint.Item2, p2) < 0;

            if (dir1 && dir2)
            {
                var intersect1 = PointMath.SegmentIntersectsSegment(center, p1, startPoint.Item2, p2);
                var intersect2 = PointMath.SegmentIntersectsSegment(center, p2, startPoint.Item2, p1);
                if (!intersect1 && !intersect2)
                {
                    var dist1 = PointMath.DistanceSquare(center, p1);
                    var dist2 = PointMath.DistanceSquare(center, p2);
                    return dist1 < dist2 ? (startPoint.Item1, p1) : (startPoint.Item1, p2);
                }
                else if (!intersect2)
                {
                    return (startPoint.Item1, p2);
                }
                else if (!intersect1)
                {
                    return (startPoint.Item1, p1);
                }
                else
                {
                    throw new Exception("Cant decide which point is better.");
                }
            }
            else if (dir1)
            {
                return (startPoint.Item1, p1);
            }
            else if (dir2)
            {
                return (startPoint.Item1, p2);
            }

            return null;
        }

    }
}