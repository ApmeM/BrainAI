using System;

namespace BrainAI.Pathfinding
{
    public static class PointMath
    {
        public static bool IsDirectionInsidePolygon(Point point, Point point2, Point pointPrev, Point pointNext, bool concave)
        {
            var leftAngle = (point2 - point).Cross(pointPrev - point);
            var rightAngle = (pointNext - point).Cross(point2 - point);

            return !concave && Math.Sign(leftAngle) == Math.Sign(rightAngle) && Math.Sign(leftAngle) == 1 ||
                    concave && !(Math.Sign(leftAngle) == Math.Sign(rightAngle) && Math.Sign(leftAngle) == -1);
        }

        // This method is taken from FingerMath
        public static bool SegmentIntersectsSegment(Point p11, Point p12, Point p21, Point p22)
        {
            var a = p12 - p11;
            var b = p21 - p22;
            var d = p21 - p11;

            var det = a.Cross(b);
            var r = d.Cross(b);
            var s = a.Cross(d);

            if (det > 0)
                return !(r < 0 || r > det || s < 0 || s > det);
            if (det < 0)
                return !(-r < 0 || -r > -det || -s < 0 || -s > -det);
            return false;            
        }

        public static (int, Point)? FindEndPoint(Point center, (int, Point) startPoint, Point p1, Point p2)
        {
            var dir1 = (p1 - startPoint.Item2).Cross(center - startPoint.Item2) < 0;
            var dir2 = (p2 - startPoint.Item2).Cross(center - startPoint.Item2) < 0;

            if (dir1 && dir2)
            {
                var intersect1 = PointMath.SegmentIntersectsSegment(center, p1, startPoint.Item2, p2);
                var intersect2 = PointMath.SegmentIntersectsSegment(center, p2, startPoint.Item2, p1);
                if (!intersect1 && !intersect2)
                {
                    var dist1 = (center - p1).LengthQuad;
                    var dist2 = (center - p2).LengthQuad;
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