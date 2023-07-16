using System;
using System.Collections.Generic;
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

            var start = new Point(0, 0);
            var end = new Point(500, 500);
            var pathData = new AStarPathfinder<Point>(graph).Search(start, end);
            Assert.AreEqual(2, pathData.Count);
        }

        [Test]
        public void SingleObstacle()
        {
            var graph = new StrightEdgeGraph();
            graph.AddPoint(1, new Point(200, 300));
            graph.AddPoint(1, new Point(1000, 300));
            graph.AddPoint(1, new Point(1000, 500));
            graph.AddPoint(1, new Point(200, 500));

            var start = new Point(100, 100);
            var end = new Point(900, 900);
            var pathData = new AStarPathfinder<Point>(graph).Search(start, end);
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

            var start = new Point(0, 0);
            var end = new Point(10, 10);
            var pathData = new AStarPathfinder<Point>(graph).Search(start, end);

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

            var start = new Point(35, 35);
            var end = new Point(100, 100);
            var pathData = new AStarPathfinder<Point>(graph).Search(start, end);

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

            var start = new Point(15, 15);
            var end = new Point(35, 15);

            var pathData = new AStarPathfinder<Point>(graph).Search(start, end);

            Assert.AreEqual(4, pathData.Count);
            Assert.AreEqual(new Point(15, 15), pathData[0]);
            Assert.AreEqual(new Point(20, 20), pathData[1]);
            Assert.AreEqual(new Point(30, 20), pathData[2]);
            Assert.AreEqual(new Point(35, 15), pathData[3]);
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

            var start = new Point(-5, 15);
            var end = new Point(55, 15);

            var pathData = new AStarPathfinder<Point>(graph).Search(start, end);

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

            var start = new Point(15, 15);
            var end = new Point(100, 100);

            var pathData = new AStarPathfinder<Point>(graph).Search(start, end);

            Assert.IsNull(pathData);
        }

        [Test]
        public void PathFindingForDifferentSizes()
        {
            for (var ArrayLength = 0; ArrayLength < 30; ArrayLength++)
            {
                var graph = new GridGraph(ArrayLength, ArrayLength, true);
                int x;
                int y;
                for (var step = 0; step < ArrayLength / 4 - 1; step++)
                {
                    x = step * 4;
                    for (y = x + 1; y < ArrayLength - 1; y++)
                    {
                        graph.Walls.Add(new Point(x, y));
                        graph.Walls.Add(new Point(x + 1, y));
                    }

                    y = step * 4 + 2;
                    for (x = y + 1; x < ArrayLength - 1; x++)
                    {
                        graph.Walls.Add(new Point(x, y));
                        graph.Walls.Add(new Point(x, y + 1));
                    }
                }

                var strightEdge = new StrightEdgeGraph();
                var pathfinder = new AStarPathfinder<Point>(strightEdge);
                GridToStrightEdgeConverter.Default.BuildGraph(graph, strightEdge);
                var pathData = pathfinder.Search(new Point(0, 0), new Point(ArrayLength - 1, ArrayLength - 1));
                Assert.NotNull(pathData);
            }
        }
    }
}
