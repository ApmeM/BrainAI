using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace BrainAI.Pathfinding
{
    public class StrightEdgeGraph : IAstarGraph<Point>
    {
        internal readonly List<StrightEdgeObstacle> obstacles = new List<StrightEdgeObstacle>();

        private Lookup<Point, Point> tempConnections = new Lookup<Point, Point>(true);

        private Lookup<Point, Point> connections = new Lookup<Point, Point>(true);

        private readonly List<Point> neighbors = new List<Point>();

        private readonly List<Point> tmpList = new List<Point>();

        // It contains points that are concave or contained to another obstacle.
        private HashSet<Point> pointsToIgnore = new HashSet<Point>();

        public void AddObstacle(List<Point> points)
        {
            this.AddObstacle(new StrightEdgeObstacle(points));
        }

        internal void AddObstacle(StrightEdgeObstacle obstacle)
        {
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


                    Log($"Point {point2} is in new obstacle.");
                    pointsToIgnore.Add(point2);

                    tmpList.Clear();
                    foreach (var connection in connections.Find(point2))
                    {
                        tmpList.Add(connection);
                    }

                    foreach (var point3 in tmpList)
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

            foreach (var pointNext in new ExtendedEnumerable<Point>(obstacle.points, obstacle.points.Count + 2))
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
                        if (
                            PointMath.DistanceSquare(obstacle2.center, point.Value) > obstacle2.radiusSq ||
                            !PointMath.PointWithinPolygon(obstacle2.points, point.Value))
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
                foreach (var point2 in obstacle2.points)
                {
                    if (pointsToIgnore.Contains(point2))
                    {
                        continue;
                    }

                    tmpList.Clear();
                    foreach (var point3 in connections.Find(point2))
                    {
                        if (PointMath.SegmentIntersectCircle(point2, point3, obstacle.center, obstacle.radiusSq) &&
                            PointMath.SegmentIntersectsPolygon(obstacle.points, point2, point3, false))
                        {
                            tmpList.Add(point3);
                        }
                    }

                    foreach (var point3 in tmpList)
                    {
                        Log($"New obstacle break connection between {point2} and {point3}");
                        connections.Remove(point2, point3);
                        connections.Remove(point3, point2);
                    }
                }
            }

            Log($"Adding new obstacle to the graph.");
            this.obstacles.Add(obstacle);

            pointPrev = null;
            point = null;
            // connect the obstacle's points with all nearby points.
            foreach (var pointNext in new ExtendedEnumerable<Point>(obstacle.points, obstacle.points.Count + 2))
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

                    Log($"New point {point} have {this.connections.Find(point.Value).Count} connections: {(string.Join(",", this.connections.Find(point.Value)))}");
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
        private Comparison<StrightEdgeObstacle> sortByDistance;

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
            wrapper.p = point;
            sortByDistance = sortByDistance ?? ((StrightEdgeObstacle a, StrightEdgeObstacle b) => (int)((PointMath.DistanceSquare(wrapper.p, a.center) - a.radiusSq) - (PointMath.DistanceSquare(wrapper.p, b.center) - b.radiusSq)));
            obstacles.Sort(sortByDistance);

            // Test the point for straight lines to points in other
            // polygons (including obstacle itself).
            foreach (var obstacle2 in obstacles)
            {
                Point? point2Prev = null;
                Point? point2 = null;
                // connect the obstacle's points with all nearby points.
                foreach (var point2Next in new ExtendedEnumerable<Point>(obstacle2.points, obstacle2.points.Count + 2))
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
                            if (PointMath.SegmentIntersectCircle(point, point2.Value, obstacle3.center, obstacle3.radiusSq) &&
                                PointMath.SegmentIntersectsPolygon(obstacle3.points, point, point2.Value, true))
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
            this.neighbors.Clear();
            foreach (var p in connections.Find(point))
            {
                this.neighbors.Add(p);
            }
            foreach (var p in tempConnections.Find(point))
            {
                this.neighbors.Add(p);
            }

            Log($"Neighbours for point {point}: {(string.Join(", ", this.neighbors))}");
            return this.neighbors;
        }

        public void BeforeSearch(Point start, HashSet<Point> ends)
        {
            this.tempConnections.Clear();
            // Connect the startpoint to its reachable points and vice versa
            this.FindConnections(start, null, null, this.tempConnections);
            Log($"For start ({start}) found {this.tempConnections.Find(start).Count} items: {string.Join(",", this.tempConnections.Find(start))}");
            foreach (var end in ends)
            {
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
                    Log($"End is achieveble from start..");
                    this.tempConnections.Add(start, end);
                    this.tempConnections.Add(end, start);
                }
                else
                {
                    // Connect the endpoint to its reachable points and vice versa
                    this.FindConnections(end, null, null, this.tempConnections);
                    Log($"For end ({end})found {this.tempConnections.Find(end).Count} items: {string.Join(",", this.tempConnections.Find(end))}");
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