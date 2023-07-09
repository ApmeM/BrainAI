namespace BrainAI.Tests
{
    using System.Collections.Generic;
    using BrainAI.Pathfinding;

    using NUnit.Framework;

    [TestFixture]
    public class GridToStrightEdgeConverterTest
    {
        [Test]
        public void BuildGraph_SinglePoint()
        {
            var grid = new GridGraph(5, 5);
            grid.Walls.Add(new Point(2, 2));
            var graph = new StrightEdgeGraph();
            GridToStrightEdgeConverter.Default.BuildGraph(grid, graph);

            CollectionAssert.AreEqual(new List<Point> { new Point(2, 2), new Point(3, 2), new Point(3, 3), new Point(2, 3) }, graph.obstacles[0]);
        }

        [Test]
        public void BuildGraph_TwoPointsUp()
        {
            var grid = new GridGraph(5, 5);
            grid.Walls.Add(new Point(2, 2));
            grid.Walls.Add(new Point(2, 3));
            var graph = new StrightEdgeGraph();
            GridToStrightEdgeConverter.Default.BuildGraph(grid, graph);
            CollectionAssert.AreEqual(new List<Point> { new Point(2, 2), new Point(3, 2), new Point(3, 4), new Point(2, 4) }, graph.obstacles[0]);
        }

        [Test]
        public void BuildGraph_TwoPointsDown()
        {
            var grid = new GridGraph(5, 5);
            grid.Walls.Add(new Point(2, 2));
            grid.Walls.Add(new Point(2, 1));
            var graph = new StrightEdgeGraph();
            GridToStrightEdgeConverter.Default.BuildGraph(grid, graph);
            CollectionAssert.AreEqual(new List<Point> { new Point(2, 1), new Point(3, 1), new Point(3, 3), new Point(2, 3) }, graph.obstacles[0]);
        }

        [Test]
        public void BuildGraph_TwoPointsRight()
        {
            var grid = new GridGraph(5, 5);
            grid.Walls.Add(new Point(2, 2));
            grid.Walls.Add(new Point(3, 2));
            var graph = new StrightEdgeGraph();
            GridToStrightEdgeConverter.Default.BuildGraph(grid, graph);
            CollectionAssert.AreEqual(new List<Point> { new Point(2, 2), new Point(4, 2), new Point(4, 3), new Point(2, 3) }, graph.obstacles[0]);
        }

        [Test]
        public void BuildGraph_TwoPointsLeft()
        {
            var grid = new GridGraph(5, 5);
            grid.Walls.Add(new Point(2, 2));
            grid.Walls.Add(new Point(1, 2));
            var graph = new StrightEdgeGraph();
            GridToStrightEdgeConverter.Default.BuildGraph(grid, graph);
            CollectionAssert.AreEqual(new List<Point> { new Point(3, 2), new Point(3, 3), new Point(1, 3), new Point(1, 2) }, graph.obstacles[0]);
        }

        [Test]
        public void BuildGraph_IndependentPoints()
        {
            var grid = new GridGraph(5, 5);
            grid.Walls.Add(new Point(2, 2));
            grid.Walls.Add(new Point(4, 2));
            var graph = new StrightEdgeGraph();
            GridToStrightEdgeConverter.Default.BuildGraph(grid, graph);
            CollectionAssert.AreEqual(new List<Point> { new Point(2, 2), new Point(3, 2), new Point(3, 3), new Point(2, 3) }, graph.obstacles[0]);
            CollectionAssert.AreEqual(new List<Point> { new Point(4, 2), new Point(5, 2), new Point(5, 3), new Point(4, 3) }, graph.obstacles[1]);
        }

        [Test]
        public void BuildGraph_Box22()
        {
            var grid = new GridGraph(5, 5);
            grid.Walls.Add(new Point(2, 2));
            grid.Walls.Add(new Point(3, 2));
            grid.Walls.Add(new Point(2, 3));
            grid.Walls.Add(new Point(3, 3));
            var graph = new StrightEdgeGraph();
            GridToStrightEdgeConverter.Default.BuildGraph(grid, graph);
            CollectionAssert.AreEqual(new List<Point> { new Point(2, 2), new Point(4, 2), new Point(4, 4), new Point(2, 4) }, graph.obstacles[0]);
        }

        [Test]
        public void BuildGraph_Scale()
        {
            var grid = new GridGraph(5, 5);
            grid.Walls.Add(new Point(2, 2));
            grid.Walls.Add(new Point(3, 2));
            grid.Walls.Add(new Point(2, 3));
            grid.Walls.Add(new Point(3, 3));
            var graph = new StrightEdgeGraph();
            GridToStrightEdgeConverter.Default.BuildGraph(grid, graph, 10);
            CollectionAssert.AreEqual(new List<Point> { new Point(20, 20), new Point(40, 20), new Point(40, 40), new Point(20, 40) }, graph.obstacles[0]);
        }

        [Test]
        public void BuildGraph_Complex()
        {
            var grid = new GridGraph(5, 5);
            grid.Walls.Add(new Point(2, 2));
            grid.Walls.Add(new Point(3, 2));
            grid.Walls.Add(new Point(2, 3));
            grid.Walls.Add(new Point(3, 3));
            grid.Walls.Add(new Point(4, 3));
            grid.Walls.Add(new Point(2, 4));
            grid.Walls.Add(new Point(3, 4));
            grid.Walls.Add(new Point(4, 4));
            grid.Walls.Add(new Point(2, 5));
            grid.Walls.Add(new Point(3, 5));
            grid.Walls.Add(new Point(3, 6));
            grid.Walls.Add(new Point(2, 7));
            grid.Walls.Add(new Point(3, 7));
            var graph = new StrightEdgeGraph();
            GridToStrightEdgeConverter.Default.BuildGraph(grid, graph);
            CollectionAssert.AreEqual(new List<Point> { new Point(2, 2), new Point(4, 2), new Point(4, 3), new Point(5, 3), new Point(5, 5), new Point(4, 5), new Point(4, 8), new Point(2, 8), new Point(2, 7), new Point(3, 7), new Point(3, 6), new Point(2, 6) }, graph.obstacles[0]);
        }
    }
}
