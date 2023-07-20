using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace BrainAI.Pathfinding
{
    public class StrightEdgeGraph : IAstarGraph<Point>
    {
        private readonly List<(int, Point)> points = new List<(int, Point)>();
        private readonly Dictionary<(int, Point), (Point, Point)> obstacleConnections = new Dictionary<(int, Point), (Point, Point)>();
        private readonly HashSet<(int, Point)> isConcave = new HashSet<(int, Point)>();
        private readonly HashSet<((int, Point), (int, Point))> unfinishedLines = new HashSet<((int, Point), (int, Point))>();
        private readonly List<((int, Point), (int, Point))> linesToRemove = new List<((int, Point), (int, Point))>();

        private readonly List<(int, Point)> candidates = new List<(int, Point)>();

        internal readonly Lookup<int, Point> obstacles = new Lookup<int, Point>();
        private readonly HashSet<int> obstacleDirty = new HashSet<int>();
        private readonly Lookup<Point, Point> connections = new Lookup<Point, Point>(true);

        // Following temp* fields are temporal and are cleared before usage.
        private readonly Lookup<Point, Point> tempConnections = new Lookup<Point, Point>(true);
        private readonly List<Point> tempNeighbors = new List<Point>();
        private readonly List<Point> tempList = new List<Point>();
        private PointWrapper wrapper = new PointWrapper();
        private Comparison<(int, Point)> sortByAngleFromPoint;

        private class PointWrapper
        {
            public Point p;
        }

        public StrightEdgeGraph()
        {
            sortByAngleFromPoint = (((int, Point) first, (int, Point) second) =>
            {
                 var result = (first.Item2-wrapper.p).CompareTo(second.Item2-wrapper.p);
                if (result == 0)
                {
                    result = Math.Sign((wrapper.p - first.Item2).LengthQuad - (wrapper.p - second.Item2).LengthQuad);
                }
                return result;

            });

        }

        public void Clear()
        {
            points.Clear();
            obstacleDirty.Clear();
            obstacles.Clear();

            obstacleConnections.Clear();
            isConcave.Clear();
            connections.Clear();
        }

        public void AddPoint(int obstacle, Point point)
        {
            this.points.Add((obstacle, point));
            this.obstacleDirty.Add(obstacle);
            this.obstacles.Add(obstacle, point);
        }

        private void ApplyChanges()
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

                Point? pointPrev = null;
                Point? point = null;

                var totalAreaX2 = 0d;
                var basePoint = new Point(0, 0);
                foreach (var pointNext in new ExtendedEnumerable<Point>(obstacles[obstacle], obstacles[obstacle].Count + 2))
                {
                    try
                    {
                        if (pointPrev == null)
                        {
                            continue;
                        }

                        obstacleConnections[(obstacle, point.Value)] = (pointPrev.Value, pointNext);

                        var triangleSquare = (pointNext-pointPrev.Value).Cross(point.Value-pointPrev.Value);
                        totalAreaX2 += triangleSquare;

                        if (triangleSquare > 0)
                            // Polygon is CCW - it is checked below
                            // But the points is CW, which means the point is concave
                            isConcave.Add((obstacle, point.Value));
                        else
                            isConcave.Remove((obstacle, point.Value));
                    }
                    finally
                    {
                        pointPrev = point;
                        point = pointNext;
                    }
                }

                if (totalAreaX2 > 0)
                {
                    // ToDo: reverse;
                    throw new Exception($"Points should be in couter clockwise order.");
                }
            }

            this.obstacleDirty.Clear();
            this.connections.Clear();
        }

        private void FindConnections(Point p, Lookup<Point, Point> reachablepoints)
        {
            Log($"=-=-=-=-=-=-=-=");
            wrapper.p = p;
            points.Sort(sortByAngleFromPoint);
            unfinishedLines.Clear();
            candidates.Clear();
            Log($"Finding connections for point {p}. Sorted list of points around: {string.Join(", ", points.Select(a => a.Item2))}");
            foreach (var point2 in points)
            {
                var invalidCandidate =
                    point2.Item2 == p ||
                    isConcave.Contains(point2) ||
                    PointMath.IsDirectionInsidePolygon(point2.Item2, p, obstacleConnections[point2].Item1, obstacleConnections[point2].Item2, isConcave.Contains(point2));
                LogIf(invalidCandidate, $"-- {point2.Item2} is either same, concave or directed inside polygon.");

                var nextPoint = PointMath.FindEndPoint(p, point2, obstacleConnections[point2].Item1, obstacleConnections[point2].Item2);
                LogIf(nextPoint != null, $"-- {point2.Item2} is the starting point for segment {(point2.Item2, nextPoint?.Item2)}. Number of segments left: {unfinishedLines.Count}");

                linesToRemove.Clear();
                foreach (var segment in unfinishedLines)
                {
                    if (segment.Item2 == point2)
                    {
                        // We found end of the unfinished line - remove line from hash.
                        // No need to check intersection of this point with this segment.
                        linesToRemove.Add(segment);
                        continue;
                    }

                    var segmentIntersectPoint2 = PointMath.SegmentIntersectsSegment(segment.Item1.Item2, segment.Item2.Item2, p, point2.Item2);
                    LogIf(segmentIntersectPoint2, $"-- {point2.Item2} to {p} vector intersected with unfinished line {(segment.Item1.Item2, segment.Item2.Item2)}");

                    if (nextPoint != null && segmentIntersectPoint2 && PointMath.SegmentIntersectsSegment(segment.Item1.Item2, segment.Item2.Item2, p, nextPoint.Value.Item2))
                    {
                        Log($"-- {point2.Item2} to {nextPoint.Value.Item2} line is fully blocked by segment  {(segment.Item1.Item2, segment.Item2.Item2)}.");
                        nextPoint = null;
                    }

                    invalidCandidate = invalidCandidate || segmentIntersectPoint2;
                }

                if (!invalidCandidate)
                {
                    Log($"-- {point2.Item2} is a new candidate. No intersection found between {p} and {point2.Item2}.");
                    candidates.Add(point2);
                }

                foreach (var lineToRemove in linesToRemove)
                {
                    unfinishedLines.Remove(lineToRemove);
                    Log($"-- {point2.Item2} is the final point for segment {(lineToRemove.Item1.Item2, lineToRemove.Item2.Item2)}. Number of segments left: {unfinishedLines.Count}");
                }

                if (nextPoint != null)
                {
                    unfinishedLines.Add((point2, nextPoint.Value));
                }
            }

            Log($"Found {candidates.Count} candidates:{(string.Join(",", candidates))}");
            // After loop through all points we might still have unfinished lines.
            // Need to check all potential connections for those lines.
            // Also check that those candidates are not directed inside polygon.
            foreach (var point in candidates)
            {
                var intersectionFound = false;
                foreach (var segment in unfinishedLines)
                {
                    if (segment.Item2 == point || segment.Item1 == point)
                    {
                        // Here both start and end of the unfinished line is a valid reachable point.
                        continue;
                    }

                    if (PointMath.SegmentIntersectsSegment(segment.Item1.Item2, segment.Item2.Item2, p, point.Item2))
                    {
                        intersectionFound = true;
                        Log($"-- {point.Item2} intersected with unfinished line {(segment.Item1.Item2, segment.Item2.Item2)}.");
                        break;
                    }
                }

                if (!intersectionFound)
                {
                    reachablepoints.Add(p, point.Item2);
                }
            }

            Log($"For point {p} found {reachablepoints[p].Count} connections: {string.Join(",", reachablepoints[p])}");
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
            foreach (var p in tempConnections[point])
            {
                this.tempNeighbors.Add(p);
            }

            if (!connections.Contains(point) && point != start && !ends.Contains(point))
            {
                FindConnections(point, connections);
            }

            foreach (var p in connections[point])
            {
                this.tempNeighbors.Add(p);
            }

            Log($"Neighbours for point {point}: {(string.Join(", ", this.tempNeighbors))}");
            return this.tempNeighbors;
        }
        private Point start;
        private HashSet<Point> ends;
        public void BeforeSearch(Point start, HashSet<Point> ends)
        {
            this.start = start;
            this.ends = ends;

            this.ApplyChanges();
            this.tempConnections.Clear();
            // Connect the startpoint to its reachable points and vice versa
            this.FindConnections(start, this.tempConnections);
            foreach (var end in ends)
            {
                if (this.points.Count == 0)
                {
                    // No obstacles on a map. Join start and end point directly.
                    this.tempConnections.Add(start, end);
                    this.tempConnections.Add(end, start);
                    continue;
                }

                // ToDo: end is achievable from start.

                // Connect the endpoint to its reachable points and vice versa
                this.FindConnections(end, this.tempConnections);

                tempList.Clear();
                foreach (var point in tempConnections[end])
                {
                    tempList.Add(point);
                }
                foreach (var p in tempList)
                {
                    tempConnections.Add(p, end);
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

        [Conditional("DEBUG")]
        private void LogIf(bool condition, string text)
        {
            if (condition)
            {
                Log(text);
            }
        }
    }
}