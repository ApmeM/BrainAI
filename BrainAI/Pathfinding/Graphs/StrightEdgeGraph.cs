using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace BrainAI.Pathfinding
{
    public class StrightEdgeGraph : IAstarGraph<Point>
    {
        internal readonly Lookup<int, Point> obstacles = new Lookup<int, Point>();
        internal readonly Dictionary<int, Point> obstacleCenter = new Dictionary<int, Point>();
        internal readonly Dictionary<int, double> obstacleRadiusSquare = new Dictionary<int, double>();
        internal readonly HashSet<int> obstacleDirty = new HashSet<int>();

        // It contains points that are concave or contained to another obstacle.
        private HashSet<Point> pointsToIgnore = new HashSet<Point>();

        private readonly Lookup<Point, Point> connections = new Lookup<Point, Point>(true);

        // Following temp* fields are temporal and are cleared before usage.
        private readonly Lookup<Point, Point> tempConnections = new Lookup<Point, Point>(true);
        private readonly List<Point> tempNeighbors = new List<Point>();
        private readonly List<Point> tempList = new List<Point>();

        public void Clear()
        {
            obstacles.Clear();
            obstacleCenter.Clear();
            obstacleRadiusSquare.Clear();
            obstacleDirty.Clear();
            pointsToIgnore.Clear();
            connections.Clear();
        }

        public void AddPoint(int obstacle, Point point)
        {
            this.obstacleDirty.Add(obstacle);
            this.obstacles.Add(obstacle, point);
        }

        public void ApplyChanges()
        {
            if (this.obstacleDirty.Count == 0)
            {
                return;
            }

            foreach (var obstacle in this.obstacleDirty)
            {
                var pointsList = obstacles[obstacle];
                if (pointsList.Count < 3)
                {
                    throw new Exception($"Minimum of 3 points needed. pointsList.Count == {pointsList.Count}");
                }

                var (centerPoint, ccw) = PointMath.CalcCenterOfPolygon(pointsList);
                this.obstacleCenter[obstacle] = centerPoint;
                this.obstacleRadiusSquare[obstacle] = PointMath.CalcRadiusSquare(pointsList, centerPoint);

                if (!ccw)
                {
                    // ToDo: reverse;
                    throw new Exception($"Points should be in couter clockwise order.");
                }
            }

            this.obstacleDirty.Clear();
            this.connections.Clear();
            this.pointsToIgnore.Clear();
            foreach (var obstacle in this.obstacles)
            {
                UpdateObstacle(obstacle.Key);
            }
        }

        public void UpdateObstacle(int obstacle)
        {
            // Any points that may be contained need to be marked as so.
            foreach (var obstacle2 in obstacles)
            {
                if (obstacle2.Key == obstacle)
                {
                    continue;
                }

                foreach (var point2 in obstacle2)
                {
                    if (
                        pointsToIgnore.Contains(point2) ||
                        PointMath.DistanceSquare(obstacleCenter[obstacle], point2) > obstacleRadiusSquare[obstacle] ||
                        !PointMath.PointWithinPolygon(obstacles[obstacle], point2))
                    {
                        continue;
                    }


                    Log($"Point {point2} is in new obstacle.");
                    pointsToIgnore.Add(point2);

                    tempList.Clear();
                    foreach (var connection in connections[point2])
                    {
                        tempList.Add(connection);
                    }

                    foreach (var point3 in tempList)
                    {
                        Log($"New obstacle break connection between {point2} and {point3}");
                        connections.Remove(point2, point3);
                        connections.Remove(point3, point2);
                    }
                }
            }

            // Find points that are concave or contained in another obstacle.
            Point? pointPrev = null;
            Point? point = null;

            foreach (var pointNext in new ExtendedEnumerable<Point>(obstacles[obstacle], obstacles[obstacle].Count + 2))
            {
                try
                {
                    if (pointPrev == null)
                    {
                        continue;
                    }

                    if (PointMath.DoubledTriangleSquareBy3Dots(point.Value, pointPrev.Value, pointNext) > 0)
                    {
                        // Polygon is CCW - it is checked in constructor
                        // But the points is CW, which means the point is concave
                        pointsToIgnore.Add(point.Value);
                        continue;
                    }

                    foreach (var obstacle2 in this.obstacles)
                    {
                        // Current obstacle definitely has its points. No need to check it as all points will be ignored.
                        if (obstacle2.Key == obstacle)
                        {
                            continue;
                        }

                        if (PointMath.DistanceSquare(obstacleCenter[obstacle2.Key], point.Value) > obstacleRadiusSquare[obstacle2.Key] ||
                            !PointMath.PointWithinPolygon(obstacle2, point.Value))
                        {
                            continue;
                        }

                        Log($"New point {point} is in old obstacle.");
                        pointsToIgnore.Add(point.Value);
                        break;
                    }
                }
                finally
                {
                    pointPrev = point;
                    point = pointNext;
                }
            }

            // Check if the new obstacle obstructs any point connections, and if it does, delete them.
            foreach (var obstacle2 in obstacles)
            {
                // Current obstacle has no connections yet, no need to check it.
                if (obstacle2.Key == obstacle)
                {
                    continue;
                }

                foreach (var point2 in obstacle2)
                {
                    if (pointsToIgnore.Contains(point2))
                    {
                        continue;
                    }

                    tempList.Clear();
                    foreach (var point3 in connections[point2])
                    {
                        if (PointMath.SegmentIntersectCircle(point2, point3, obstacleCenter[obstacle], obstacleRadiusSquare[obstacle]) &&
                            PointMath.SegmentIntersectsPolygon(obstacles[obstacle], point2, point3, false))
                        {
                            tempList.Add(point3);
                        }
                    }

                    foreach (var point3 in tempList)
                    {
                        Log($"New obstacle break connection between {point2} and {point3}");
                        connections.Remove(point2, point3);
                        connections.Remove(point3, point2);
                    }
                }
            }

            pointPrev = null;
            point = null;
            // connect the obstacle's points with all nearby points.
            foreach (var pointNext in new ExtendedEnumerable<Point>(obstacles[obstacle], obstacles[obstacle].Count + 2))
            {
                try
                {
                    if (pointPrev == null)
                    {
                        continue;
                    }

                    if (pointsToIgnore.Contains(point.Value))
                    {
                        continue;
                    }

                    FindConnections(point.Value, pointPrev, pointNext, this.connections);

                    Log($"New point {point} have {this.connections[point.Value].Count} connections: {(string.Join(",", this.connections[point.Value]))}");
                }
                finally
                {
                    pointPrev = point;
                    point = pointNext;
                }
            }
            Log("Total connections: " + this.connections.Sum(a => ((Lookup<Point, Point>.Enumerable)a).Count));
            Log(string.Join("\n", this.connections.Select(a => $"From {a.Key} to " + string.Join(",", a))));
        }
        private class PointWrapper
        {
            public Point p;
        }
        private PointWrapper wrapper = new PointWrapper();
        private Comparison<int> sortByDistance;

        private void FindConnections(Point point, Point? pointPrev, Point? pointNext, Lookup<Point, Point> reachablepoints)
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

            // ToDo: sort
            // wrapper.p = point;
            // sortByDistance = sortByDistance ?? ((int a, int b) => (int)((PointMath.DistanceSquare(wrapper.p, obstacleCenter[a]) - obstacleRadiusSquare[a]) - (PointMath.DistanceSquare(wrapper.p, obstacleCenter[b]) - obstacleRadiusSquare[b])));
            // obstacles.Sort(sortByDistance);

            // Test the point for straight lines to points in other
            // polygons (including obstacle itself).
            foreach (var obstacle2 in obstacles)
            {
                Point? point2Prev = null;
                Point? point2 = null;
                // connect the obstacle's points with all nearby points.
                foreach (var point2Next in new ExtendedEnumerable<Point>(obstacle2, obstacle2.Count + 2))
                {
                    try
                    {
                        if (point2Prev == null)
                        {
                            continue;
                        }

                        if (point2 == point)
                        {
                            Log($"Skipping segment {point} - {point2}: Same point");
                            continue;
                        }
                        if (pointsToIgnore.Contains(point2.Value))
                        {
                            Log($"Skipping segment {point} - {point2}: Point {point2} is in ignore list");
                            continue;
                        }
                        if (PointMath.IsDirectionInsidePolygon(point2.Value, point, point2Prev.Value, point2Next))
                        {
                            Log($"Skipping segment {point} - {point2}: Directed inside poligon for {point2}");
                            continue;
                        }
                        if (pointPrev != null && pointNext != null &&
                            PointMath.IsDirectionInsidePolygon(point, point2.Value, pointPrev.Value, pointNext.Value))
                        {
                            Log($"Skipping segment {point} - {point2}: Directed inside poligon for {point}");
                            continue;
                        }

                        // Need to test if line from point to point2 intersects any obstacles
                        var found = true;
                        foreach (var obstacle3 in obstacles)
                        {
                            if (PointMath.SegmentIntersectCircle(point, point2.Value, obstacleCenter[obstacle3.Key], obstacleRadiusSquare[obstacle3.Key]) &&
                                PointMath.SegmentIntersectsPolygon(obstacle3, point, point2.Value, true))
                            {
                                Log($"Checking segment {point} - {point2}: Another obstacle intersects.");
                                found = false;
                                break;
                            }
                        }

                        if (found)
                        {
                            reachablepoints.Add(point, point2.Value);
                            reachablepoints.Add(point2.Value, point);
                        }
                    }
                    finally
                    {
                        point2Prev = point2;
                        point2 = point2Next;

                    }
                }
            }
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
            this.tempNeighbors.Clear();
            foreach (var p in connections[point])
            {
                this.tempNeighbors.Add(p);
            }
            foreach (var p in tempConnections[point])
            {
                this.tempNeighbors.Add(p);
            }

            Log($"Neighbours for point {point}: {(string.Join(", ", this.tempNeighbors))}");
            return this.tempNeighbors;
        }

        public void BeforeSearch(Point start, HashSet<Point> ends)
        {
            this.ApplyChanges();
            this.tempConnections.Clear();
            // Connect the startpoint to its reachable points and vice versa
            this.FindConnections(start, null, null, this.tempConnections);
            Log($"For start ({start}) found {this.tempConnections[start].Count} items: {string.Join(",", this.tempConnections[start])}");
            foreach (var end in ends)
            {
                var found = false;
                foreach (var obstacle in this.obstacles)
                {
                    if (PointMath.SegmentIntersectCircle(start, end, obstacleCenter[obstacle.Key], obstacleRadiusSquare[obstacle.Key]) &&
                        PointMath.SegmentIntersectsPolygon(obstacle, start, end, false))
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    Log($"End is achieveble from start..");
                    this.tempConnections.Add(start, end);
                    this.tempConnections.Add(end, start);
                }
                else
                {
                    // Connect the endpoint to its reachable points and vice versa
                    this.FindConnections(end, null, null, this.tempConnections);
                    Log($"For end ({end})found {this.tempConnections[end].Count} items: {string.Join(",", this.tempConnections[end])}");
                }
            }
            Log($"Total temp connections: {this.tempConnections.Sum(a => ((Lookup<Point, Point>.Enumerable)a).Count)}");
            Log(string.Join("\n", this.tempConnections.Select(a => $"From {a.Key} to " + string.Join(",", a))));

            Log("-=-=-=-=-=-=-");
        }
        public bool needLog = false;
        [Conditional("DEBUG")]
        private void Log(string text)
        {
            if (needLog)
                Console.WriteLine(text);
        }
    }
}