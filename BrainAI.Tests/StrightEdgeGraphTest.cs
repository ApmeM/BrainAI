using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace BrainAI.Pathfinding
{
    [TestFixture]
    public class StrightEdgeGraphTest
    {
        [Test]
        public void WithoutObstacles()
        {
            var graph = new StrightEdgeGraph();

            var pathData = DoSearch_UsedInReadme(graph, new Point(0, 0), new Point(500, 500));

            Assert.AreEqual(2, pathData.Count);
        }

        [Test]
        public void WithoutObstaclesButVisible()
        {
            // Create a graph with 4 points of the single obstacle
            var graph = new StrightEdgeGraph();
            graph.AddPoint(1, new Point(200, 300));
            graph.AddPoint(1, new Point(1000, 300));
            graph.AddPoint(1, new Point(1000, 500));
            graph.AddPoint(1, new Point(200, 500));

            var pathData = DoSearch_UsedInReadme(graph, new Point(-100, -100), new Point(-900, -900));

            Assert.AreEqual(2, pathData.Count);
            Assert.AreEqual(new Point(-100, -100), pathData[0]);
            Assert.AreEqual(new Point(-900, -900), pathData[1]);
        }


        [Test]
        public void SingleObstacle_UsedInReadme()
        {
            // Create a graph with 4 points of the single obstacle
            var graph = new StrightEdgeGraph();
            graph.AddPoint(1, new Point(200, 300));
            graph.AddPoint(1, new Point(1000, 300));
            graph.AddPoint(1, new Point(1000, 500));
            graph.AddPoint(1, new Point(200, 500));

            var pathData = DoSearch_UsedInReadme(graph, new Point(100, 100), new Point(900, 900));

            Assert.AreEqual(3, pathData.Count);
            Assert.AreEqual(new Point(100, 100), pathData[0]);
            Assert.AreEqual(new Point(200, 500), pathData[1]);
            Assert.AreEqual(new Point(900, 900), pathData[2]);
        }

        [Test]
        public void WithPathThroughAllObstacles()
        {
            var graph = new StrightEdgeGraph();

            for (var j = 0; j < 2; j++)
            {
                graph.AddPoint(j * 2, new Point(j * 4 + 0 + 0, j * 4 + 0 + 1));
                graph.AddPoint(j * 2, new Point(j * 4 + 2 + 0, j * 4 + 0 + 1));
                graph.AddPoint(j * 2, new Point(j * 4 + 2 + 0, j * 4 + 200 + 1));
                graph.AddPoint(j * 2, new Point(j * 4 + 0 + 0, j * 4 + 200 + 1));

                graph.AddPoint(j * 2 + 1, new Point(j * 4 + 0 + 3, j * 4 + 0 + 2));
                graph.AddPoint(j * 2 + 1, new Point(j * 4 + 200 + 3, j * 4 + 0 + 2));
                graph.AddPoint(j * 2 + 1, new Point(j * 4 + 200 + 3, j * 4 + 2 + 2));
                graph.AddPoint(j * 2 + 1, new Point(j * 4 + 0 + 3, j * 4 + 2 + 2));
            }

            var pathData = DoSearch_UsedInReadme(graph, new Point(0, 0), new Point(10, 10));

            Assert.AreEqual(6, pathData.Count);
            Assert.AreEqual(new Point(0, 0), pathData[0]);
            Assert.AreEqual(new Point(2, 1), pathData[1]);
            Assert.AreEqual(new Point(3, 4), pathData[2]);
            Assert.AreEqual(new Point(6, 5), pathData[3]);
            Assert.AreEqual(new Point(7, 8), pathData[4]);
            Assert.AreEqual(new Point(10, 10), pathData[5]);
        }

        [Test]
        public void ExitFromCircle()
        {
            var grid = new GridGraph(5, 5);
            grid.Walls.Add(new Point(2, 2));
            grid.Walls.Add(new Point(3, 2));
            grid.Walls.Add(new Point(4, 2));
            grid.Walls.Add(new Point(4, 3));
            grid.Walls.Add(new Point(4, 4));
            grid.Walls.Add(new Point(3, 4));
            grid.Walls.Add(new Point(2, 4));
            grid.Walls.Add(new Point(2, 3));
            var graph = new StrightEdgeGraph();
            GridToStrightEdgeConverter.Default.BuildGraph(grid, graph, 10);
            Assert.AreEqual(1, graph.obstacles.Count);
            Assert.AreEqual(10, graph.obstacles[0].Count);

            var pathData = DoSearch_UsedInReadme(graph, new Point(35, 35), new Point(100, 100));

            Assert.IsNull(pathData);
        }

        [Test]
        public void PathInsideObstacles()
        {
            var grid = new GridGraph(5, 5);
            grid.Walls.Add(new Point(0, 0));
            grid.Walls.Add(new Point(0, 1));
            grid.Walls.Add(new Point(0, 2));
            grid.Walls.Add(new Point(0, 3));
            grid.Walls.Add(new Point(1, 3));
            grid.Walls.Add(new Point(2, 3));
            grid.Walls.Add(new Point(3, 3));
            grid.Walls.Add(new Point(4, 3));
            grid.Walls.Add(new Point(4, 2));
            grid.Walls.Add(new Point(4, 1));
            grid.Walls.Add(new Point(4, 0));
            grid.Walls.Add(new Point(3, 0));
            grid.Walls.Add(new Point(2, 0));
            grid.Walls.Add(new Point(1, 0));
            grid.Walls.Add(new Point(2, 1));
            var graph = new StrightEdgeGraph();

            GridToStrightEdgeConverter.Default.BuildGraph(grid, graph, 10);
            Assert.AreEqual(1, graph.obstacles.Count);

            var pathData = DoSearch_UsedInReadme(graph, new Point(15, 15), new Point(35, 25));

            Assert.AreEqual(3, pathData.Count);
            Assert.AreEqual(new Point(15, 15), pathData[0]);
            Assert.AreEqual(new Point(20, 20), pathData[1]);
            Assert.AreEqual(new Point(35, 25), pathData[2]);
        }

        [Test]
        public void PathOutsideObstacles()
        {
            var grid = new GridGraph(5, 5);
            grid.Walls.Add(new Point(0, 0));
            grid.Walls.Add(new Point(0, 1));
            grid.Walls.Add(new Point(0, 2));
            grid.Walls.Add(new Point(0, 3));
            grid.Walls.Add(new Point(1, 3));
            grid.Walls.Add(new Point(2, 3));
            grid.Walls.Add(new Point(3, 3));
            grid.Walls.Add(new Point(4, 3));
            grid.Walls.Add(new Point(4, 2));
            grid.Walls.Add(new Point(4, 1));
            grid.Walls.Add(new Point(4, 0));
            grid.Walls.Add(new Point(3, 0));
            grid.Walls.Add(new Point(2, 0));
            grid.Walls.Add(new Point(1, 0));
            grid.Walls.Add(new Point(2, 1));
            var graph = new StrightEdgeGraph();
            GridToStrightEdgeConverter.Default.BuildGraph(grid, graph, 10);
            Assert.AreEqual(1, graph.obstacles.Count);

            var pathData = DoSearch_UsedInReadme(graph, new Point(-5, 15), new Point(55, 15));

            Assert.AreEqual(4, pathData.Count);
            Assert.AreEqual(new Point(-5, 15), pathData[0]);
            Assert.AreEqual(new Point(0, 0), pathData[1]);
            Assert.AreEqual(new Point(50, 0), pathData[2]);
            Assert.AreEqual(new Point(55, 15), pathData[3]);
        }

        [Test]
        public void PathToOutsideObstacles()
        {
            var grid = new GridGraph(5, 5);
            grid.Walls.Add(new Point(0, 0));
            grid.Walls.Add(new Point(0, 1));
            grid.Walls.Add(new Point(0, 2));
            grid.Walls.Add(new Point(0, 3));
            grid.Walls.Add(new Point(1, 3));
            grid.Walls.Add(new Point(2, 3));
            grid.Walls.Add(new Point(3, 3));
            grid.Walls.Add(new Point(4, 3));
            grid.Walls.Add(new Point(4, 2));
            grid.Walls.Add(new Point(4, 1));
            grid.Walls.Add(new Point(4, 0));
            grid.Walls.Add(new Point(3, 0));
            grid.Walls.Add(new Point(2, 0));
            grid.Walls.Add(new Point(1, 0));
            grid.Walls.Add(new Point(2, 1));
            var graph = new StrightEdgeGraph();

            GridToStrightEdgeConverter.Default.BuildGraph(grid, graph, 10);
            Assert.AreEqual(1, graph.obstacles.Count);

            var pathData = DoSearch_UsedInReadme(graph, new Point(15, 15), new Point(100, 100));

            Assert.IsNull(pathData);
        }

        [Test]
        public void PathFindingForDifferentSizes()
        {
            for (var ArrayLength = 10; ArrayLength < 30; ArrayLength++)
            {
                var gridGraph = new GridGraph(ArrayLength, ArrayLength, true);
                int x;
                int y;
                for (var step = 0; step < ArrayLength / 4 - 1; step++)
                {
                    x = step * 4;
                    for (y = x + 1; y < ArrayLength - 1; y++)
                    {
                        gridGraph.Walls.Add(new Point(x, y));
                        gridGraph.Walls.Add(new Point(x + 1, y));
                    }

                    y = step * 4 + 2;
                    for (x = y + 1; x < ArrayLength - 1; x++)
                    {
                        gridGraph.Walls.Add(new Point(x, y));
                        gridGraph.Walls.Add(new Point(x, y + 1));
                    }
                }

                var graph = new StrightEdgeGraph();
                GridToStrightEdgeConverter.Default.BuildGraph(gridGraph, graph);

                var pathData = DoSearch_UsedInReadme(graph, new Point(0, 0), new Point(ArrayLength - 1, ArrayLength - 1));

                Assert.AreEqual(4, pathData.Count);
            }
        }

        private List<Point> DoSearch_UsedInReadme(StrightEdgeGraph graph, Point start, Point end)
        {
            var result = new List<Point>();
            // Check if end is visible from start.
            if (graph.IsVisible(start, end))
            {
                result.Add(start);
                result.Add(end);
                return result;
            }

            // Find closest visible start point to start from.
            HashSet<Point> starts = new HashSet<Point>();
            graph.FindVisiblePoints(start, starts);
            // Find all visible nodes to end point.
            HashSet<Point> ends = new HashSet<Point>();
            graph.FindVisiblePoints(end, ends);
            if (!starts.Any() || !ends.Any())
            {
                // It might happen that there are no visible points for the following reasons:
                // 1. Graph is empty. In this case start and end are directly connected.
                // 2. If the rounding walls looks like well (all visible points are concave).
                return null;
            }

            // Do the search.
            // WARNING: Do not use Astar here as AStar is not really multigoal search as it have a heuristics calculations based on a single target. Instead it took first goal from set and tries to get to it. 
            // If you want to use AStar here - please provide the exact end goal point (e.g. find the closest points from all the visible points and use it).
            var pathfinder = new WeightedPathfinder<Point>(graph);
            pathfinder.Search(starts.OrderBy(a => (a - start).LengthQuad).First(), ends, result);
            if (result.Count == 0)
            {
                // Path not found.
                return null;
            }

            // As we start from closest start point it might happen that some further points are also visible and we can remove them from the list.
            var found = false;
            for (var i = result.Count; i > 0; i--)
            {
                if (found)
                {
                    result.RemoveAt(i - 1);
                    continue;
                }
                found = starts.Contains(result[i - 1]);
            }

            // Add start and end points if they are not on the graph.
            if (result[result.Count - 1] != end)
            {
                result.Add(end);
            }
            if (result[0] != start)
            {
                result.Insert(0, start);
            }
            return new List<Point>(result);
        }
    }
}
