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
            for (int pointIndex = 0; pointIndex < obstacle.points.Count; pointIndex++)
            {
                Point point = obstacle.points[pointIndex];
                // Find concave points
                var pointPrevIndex = (pointIndex - 1 + obstacle.points.Count) % obstacle.points.Count;
                var pointNextIndex = (pointIndex + 1) % obstacle.points.Count;

                var pointPrev = obstacle.points[pointPrevIndex];
                var pointNext = obstacle.points[pointNextIndex];

                if (PointMath.DoubledTriangleSquareBy3Dots(point, pointPrev, pointNext) > 0)
                {
                    // Polygon is CCW - it is checked in constructor
                    // But the points is CW, which means the point is concave
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

            // connect the obstacle's points with all nearby points.
            for (int pointIndex = 0; pointIndex < obstacle.points.Count; pointIndex++)
            {
                Point point = obstacle.points[pointIndex];
                if (pointsToIgnore.Contains(point))
                {
                    continue;
                }

                FindConnections(point, obstacle.points, pointIndex, this.connections);

                Log($"New point {point} have {this.connections.Find(point).Count()} connections: {(string.Join(",", this.connections.Find(point)))}");
            }
            Log("Total connections: " + this.connections.Sum(a => a.Count()));
            Log(string.Join("\n", this.connections.Select(a => $"From {a.Key} to " + string.Join(",", a))));
        }

        private void FindConnections(Point point, List<Point> pointList, int pointIndex, Lookup<Point, Point> reachablepoints)
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
                    if (point2.Equals(point))
                    {
                        // Log($"Skipping segment {point} - {point2}: Same point");
                        continue;
                    }
                    if (pointsToIgnore.Contains(point2))
                    {
                        // Log($"Skipping segment {point} - {point2}: Point {point2} is in ignore list");
                        continue;
                    }
                    if (PointMath.IsDirectionInsidePolygon(point2, point, obstace2.points, point2Index))
                    {
                        // Log($"Skipping segment {point} - {point2}: Directed inside poligon for {point2}");
                        continue;
                    }
                    if (pointList != null && PointMath.IsDirectionInsidePolygon(point, point2, pointList, pointIndex))
                    {
                        // Log($"Skipping segment {point} - {point2}: Directed inside poligon for {point}");
                        continue;
                    }
                    
                    // Need to test if line from point to point2 intersects any obstacles
                    var found = true;
                    foreach (var obstacle3 in obstacles)
                    {
                        if (PointMath.SegmentIntersectCircle(point, point2, obstacle3.center, obstacle3.radiusSq) &&
                            PointMath.SegmentIntersectsPolygon(obstacle3.points, point, point2, true))
                        {
                            // Log($"Checking segment {point} - {point2}: Another obstacle intersects.");
                            found = false;
                            break;
                        }
                    }

                    if (found)
                    {
                        reachablepoints.Add(point, point2);
                        reachablepoints.Add(point2, point);
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
            this.FindConnections(start, null, 0, this.tempConnections);
            Log($"For start ({start}) found {this.tempConnections[start].Count()} items: {string.Join(",", this.tempConnections[start])}");
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
                    this.FindConnections(end, null, 0, this.tempConnections);
                    Log($"For end ({end})found {this.tempConnections[end].Count()} items: {string.Join(",", this.tempConnections[end])}");
                }
            }
            Log($"Total temp connections: {this.tempConnections.Sum(a => a.Count())}");
            Log(string.Join("\n", this.tempConnections.Select(a => $"From {a.Key} to " + string.Join(",", a))));

            Log("-=-=-=-=-=-=-");
        }
        bool needLog = false;
        [Conditional("DEBUG")]
        private void Log(string text)
        {
            if (needLog)
                Console.WriteLine(text);
        }
    }
}