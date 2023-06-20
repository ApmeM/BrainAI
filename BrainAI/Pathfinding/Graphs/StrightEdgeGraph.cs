using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace BrainAI.Pathfinding
{
    public class StrightEdgeGraph : IAstarGraph<Point>
    {
        private double smallAmount = 0.0001;

        private readonly List<StrightEdgeObstacle> obstacles = new List<StrightEdgeObstacle>();

        private Dictionary<Point, HashSet<Point>> tempConnections = new Dictionary<Point, HashSet<Point>>();

        private Dictionary<Point, HashSet<Point>> connections = new Dictionary<Point, HashSet<Point>>();

        private readonly List<Point> neighbors = new List<Point>();

        private readonly List<Point> tmpList = new List<Point>();

        // It contains points that are concave or contained to another obstacle.
        private HashSet<Point> pointsToIgnore = new HashSet<Point>();

        public void AddObstacle(List<Point> points)
        {
            var obstacle = new StrightEdgeObstacle(points);

            // Any points that may be contained need to be marked as so.
            foreach (var obstacle2 in obstacles)
            {
                foreach (var point2 in obstacle2.points)
                {
                    if (
                        pointsToIgnore.Contains(point2) ||
                        PointMath.DistanceSquare(obstacle.center, point2) > obstacle.radiusSq ||
                        !PointMath.PointWithinPolygon(obstacle.points, point2))
                    {
                        continue;
                    }

                    pointsToIgnore.Add(point2);

                    foreach (var connection in connections[point2])
                    {
                        connections[connection].Remove(point2);
                    }

                    Log($"Point {point2} is in new obstacle.");
                    connections.Remove(point2);
                }
            }

            // Find points that are concave or contained to another obstacle.
            for (int pointIndex = 0; pointIndex < obstacle.points.Count; pointIndex++)
            {
                Point point = obstacle.points[pointIndex];
                // Find concave points
                var pointPrevIndex = (pointIndex + 1) % obstacle.points.Count;
                var pointNextIndex = (pointIndex - 1 + obstacle.points.Count) % obstacle.points.Count;

                var pointPrev = obstacle.points[pointPrevIndex];
                var pointNext = obstacle.points[pointNextIndex];

                if (PointMath.RelCCWDouble(point, pointPrev, pointNext) < 0)
                {
                    pointsToIgnore.Add(point);
                    continue;
                }

                foreach (var obstacle2 in this.obstacles)
                {
                    if (
                        PointMath.DistanceSquare(obstacle2.center, point) > obstacle2.radiusSq ||
                        !PointMath.PointWithinPolygon(obstacle2.points, point))
                    {
                        continue;
                    }

                    Log($"New point {point} is in old obstacle.");
                    pointsToIgnore.Add(point);
                    break;
                }

                if (pointsToIgnore.Contains(point))
                {
                    continue;
                }

                connections[point] = new HashSet<Point>();
            }

            // Check if the new obstacle obstructs any point connections, and if it does, delete them.
            foreach (var obstacle2 in obstacles)
            {
                foreach (var point2 in obstacle2.points)
                {
                    if (pointsToIgnore.Contains(point2))
                    {
                        continue;
                    }

                    tmpList.Clear();
                    foreach (var point3 in connections[point2])
                    {
                        if (PointMath.SegmentIntersectCircle(point2, point3, obstacle.center, obstacle.radiusSq) &&
                            PointMath.SegmentIntersectsPolygon(obstacle.points, point2, point3, true))
                        {
                            tmpList.Add(point3);
                        }
                    }

                    foreach (var point3 in tmpList)
                    {
                        Log($"New obstacle break connection between {point2} and {point3}");
                        connections[point2].Remove(point3);
                        connections[point3].Remove(point2);
                    }
                }
            }

            this.obstacles.Add(obstacle);
            Log($"Adding new obstacle to the graph.");

            // connect the obstacle's points with all nearby points.
            for (int pointIndex = 0; pointIndex < obstacle.points.Count; pointIndex++)
            {
                Point point = obstacle.points[pointIndex];
                if (pointsToIgnore.Contains(point))
                {
                    continue;
                }

                FindConnections(point, obstacle.points, pointIndex, this.connections[point]);

                Log($"New point {point} have {this.connections[point].Count} connections: {(string.Join(",", this.connections[point]))}");

                foreach (var connection in this.connections[point])
                {
                    if (!connections.ContainsKey(connection))
                    {
                        continue;
                    }
                    connections[connection].Add(point);
                }
            }
            Log("Total connections: " + this.connections.Sum(a => a.Value.Count));
            Log(string.Join("\n", this.connections.Select(a => $"From {a.Key} to " + string.Join(",", a.Value))));
        }

        private void FindConnections(Point point, List<Point> pointList, int pointIndex, HashSet<Point> reachablepoints)
        {
            // Test to see if it's ok to ignore this point since it's
            // concave (inward-pointing) or it's contained by an obstacle.
            if (pointsToIgnore.Contains(point))
            {
                return;
            }

            // To optimise the line-obstacle intersection testing, order the obstacle list
            // by their distance to the startpoint, smallest first.
            // These closer obstacles are more likely to intersect any lines from the
            // startpoint to the far away obstacle points.
            obstacles.Sort((a, b) => (int)((PointMath.DistanceSquare(point, a.center) - a.radiusSq) - (PointMath.DistanceSquare(point, b.center) - b.radiusSq)));

            // Test the point for straight lines to points in other
            // polygons (including obstacle itself).
            foreach (var obstace2 in obstacles)
            {
                for (int point2Index = 0; point2Index < obstace2.points.Count; point2Index++)
                {
                    Point point2 = obstace2.points[point2Index];
                    // Don't test a 'line' from the exact same points in the same polygon.
                    // Test to see if it's ok to ignore this point since it's
                    // concave (inward-pointing) or it's contained by an obstacle.
                    if (point2.Equals(point) || pointsToIgnore.Contains(point2))
                    {
                        continue;
                    }

                    // Only connect the points if the connection will be useful.
                    if (!IsConnectionPossibleAndUseful(point, pointList, pointIndex, point2, obstace2.points, point2Index))
                    {
                        continue;
                    }

                    if (PointMath.SegmentIntersectCircle(point, point2, obstace2.center, obstace2.radiusSq) &&
                        PointMath.SegmentIntersectsPolygon(obstace2.points, point, point2, true))
                    {
                        continue;
                    }

                    var pointTopoint2Dist = PointMath.DistanceSquare(point, point2);

                    // Need to test if line from point to point2 intersects any obstacles
                    // Also test if any point is contained in any obstacle.
                    var found = true;
                    foreach (var obstacle3 in obstacles)
                    {
                        if (obstacle3.Equals(obstace2))
                        {
                            continue;
                        }

                        var dist = PointMath.DistanceSquare(point, obstacle3.center) - obstacle3.radiusSq;
                        if (dist > pointTopoint2Dist)
                        {
                            continue;
                        }

                        if (!(PointMath.SegmentIntersectCircle(point, point2, obstacle3.center, obstacle3.radiusSq) &&
                            PointMath.SegmentIntersectsPolygon(obstacle3.points, point, point2, true)))
                        {
                            continue;
                        }

                        found = false;
                        break;
                    }

                    if (found)
                    {
                        reachablepoints.Add(point2);
                    }
                }
            }
        }

        private bool IsConnectionPossibleAndUseful(Point point, Point point2, List<Point> point2List, int point2Index)
        {
            // Only connect the points if the connection will be useful.
            // See the comment in the method makeReachablepoints for a full explanation.

            var point2PrevIndex = (point2Index + 1) % point2List.Count;
            var point2NextIndex = (point2Index - 1 + point2List.Count) % point2List.Count;

            var point2Prev = point2List[point2PrevIndex];
            var point2Next = point2List[point2NextIndex];

            var p2LessP2MinusX = point2.X - point2Prev.X;
            var p2LessP2MinusY = point2.Y - point2Prev.Y;
            var pLessP2MinusX = point.X - point2Prev.X;
            var pLessP2MinusY = point.Y - point2Prev.Y;

            var pLessP2X = point.X - point2.X;
            var pLessP2Y = point.Y - point2.Y;
            var p2PlusLessP2X = point2Next.X - point2.X;
            var p2PlusLessP2Y = point2Next.Y - point2.Y;

            if ((pLessP2MinusY * p2LessP2MinusX - pLessP2MinusX * p2LessP2MinusY) * (pLessP2Y * p2PlusLessP2X - pLessP2X * p2PlusLessP2Y) <= 0)
            {
                return true;
            }

            var dotprod = pLessP2MinusX * p2LessP2MinusX + pLessP2MinusY * p2LessP2MinusY;
            if (PointMath.DistanceSquare(point, point2Prev) - dotprod * dotprod / PointMath.DistanceSquare(point2, point2Prev) < smallAmount)
            {
                return true;
            }

            dotprod = pLessP2X * p2PlusLessP2X + pLessP2Y * p2PlusLessP2Y;
            if (PointMath.DistanceSquare(point, point2) - dotprod * dotprod / PointMath.DistanceSquare(point2Next, point2) < smallAmount)
            {
                return true;
            }

            return false;
        }

        private bool IsConnectionPossibleAndUseful(Point point, List<Point> pointList, int pointIndex, Point point2, List<Point> point2List, int point2Index)
        {
            if (pointList == null)
            {
                return IsConnectionPossibleAndUseful(point, point2, point2List, point2Index);
            }
            // test if startpoint is in the reject region of point2's obstacle
            {
                // Only connect the points if the connection will be useful.
                // See the comment in the method makeReachablepoints for a full explanation.
                var point2PrevIndex = (point2Index + 1) % point2List.Count;
                var point2NextIndex = (point2Index - 1 + point2List.Count) % point2List.Count;

                var point2Prev = point2List[point2PrevIndex];
                var point2Next = point2List[point2NextIndex];

                var p2MinusToP2RCCW = PointMath.RelCCWDouble(point, point2Prev, point2);
                var p2ToP2PlusRCCW = PointMath.RelCCWDouble(point, point2, point2Next);
                if (p2MinusToP2RCCW * p2ToP2PlusRCCW > 0)
                {
                    // To avoid floating point error problems we should only return false
                    // if p is well away from the lines. If it's close, then return
                    // true just to be safe. Returning false when the connection is
                    // actually useful is a much bigger problem than returning true
                    // and sacrificing some performance.
                    if (PointMath.PointToLineDistSq(point, point2Prev, point2) < smallAmount)
                    {
                        return true;
                    }

                    if (PointMath.PointToLineDistSq(point, point2, point2Next) < smallAmount)
                    {
                        return true;
                    }
                    // Since p is anti-clockwise to both lines p2MinusToP2 and p2ToP2Plus
                    // (or it is clockwise to both lines) then the connection betwen
                    // them will not be useful so return .
                    return false;
                }
            }
            // test if point2 is in the reject region of point1's obstacle
            {
                // Only connect the points if the connection will be useful.
                // See the comment in the method makeReachablepoints for a full explanation.
                var pointPrevIndex = (pointIndex + 1) % pointList.Count;
                var pointNextIndex = (pointIndex - 1 + pointList.Count) % pointList.Count;

                var pointPrev = pointList[pointPrevIndex];
                var pointNext = pointList[pointNextIndex];

                var pMinusToPRCCW = PointMath.RelCCWDouble(point2, pointPrev, point);
                var pToPPlusRCCW = PointMath.RelCCWDouble(point2, point, pointNext);
                if (pMinusToPRCCW * pToPPlusRCCW > 0)
                {
                    // To avoid floating point error problems we should only return false
                    // if p is well away from the lines. If it's close, then return
                    // true just to be safe. Returning false when the connection is
                    // actually useful is a much bigger problem than returning true
                    // and sacrificing some performance.
                    if (PointMath.PointToLineDistSq(point2, pointPrev, point) < smallAmount)
                    {
                        return true;
                    }

                    if (PointMath.PointToLineDistSq(point2, point, pointNext) < smallAmount)
                    {
                        return true;
                    }
                    // Since p is anti-clockwise to both lines p2MinusToP2 and p2ToP2Plus
                    // (or it is clockwise to both lines) then the connection betwen
                    // them will not be useful so return .
                    return false;
                }
            }
            return true;
        }

        public int Heuristic(Point point, Point goal)
        {
            return Math.Abs(goal.X - point.X) + Math.Abs(goal.Y - point.Y);
        }

        public int Cost(Point from, Point to)
        {
            return Math.Abs(from.X - to.X) + Math.Abs(from.Y - to.Y);
        }

        public List<Point> GetNeighbors(Point point)
        {
            this.neighbors.Clear();
            if (connections.ContainsKey(point))
            {
                this.neighbors.AddRange(connections[point]);
            }
            if (tempConnections.ContainsKey(point))
            {
                this.neighbors.AddRange(tempConnections[point]);
            }

            Log($"Neighbours for point {point}: {(string.Join(", ", this.neighbors))}");
            return this.neighbors;
        }

        public void BeforeSearch(Point start, HashSet<Point> ends)
        {
            this.tempConnections.Clear();

            this.tempConnections[start] = new HashSet<Point>();
            // Connect the startpoint to its reachable points and vice versa
            this.FindConnections(start, null, 0, this.tempConnections[start]);

            foreach (var point in this.tempConnections[start])
            {
                if (!this.tempConnections.ContainsKey(point))
                {
                    this.tempConnections[point] = new HashSet<Point>();
                }
                this.tempConnections[point].Add(start);
            }

            foreach (var end in ends)
            {
                // Connect the endpoint to its reachable points and vice versa

                this.tempConnections[end] = new HashSet<Point>();
                this.FindConnections(end, null, 0, this.tempConnections[end]);

                foreach (var point in this.tempConnections[end])
                {
                    if (!this.tempConnections.ContainsKey(point))
                    {
                        this.tempConnections[point] = new HashSet<Point>();
                    }

                    this.tempConnections[point].Add(end);
                }

                var found = false;
                foreach (var obstacle in this.obstacles)
                {

                    if (PointMath.SegmentIntersectCircle(start, end, obstacle.center, obstacle.radiusSq) &&
                        PointMath.SegmentIntersectsPolygon(obstacle.points, start, end, false))
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    this.tempConnections[start].Add(end);
                    this.tempConnections[end].Add(start);
                }
                Log($"for end found {this.tempConnections[end].Count} items.");
            }
            Log("Total temp connections: " + this.tempConnections.Sum(a => a.Value.Count));
            Log(string.Join("\n", this.tempConnections.Select(a => $"From {a.Key} to " + string.Join(",", a.Value))));

            Log("-=-=-=-=-=-=-");
        }

        [Conditional("DEBUG")]
        private void Log(string text)
        {
            Console.WriteLine(text);
        }
    }
}