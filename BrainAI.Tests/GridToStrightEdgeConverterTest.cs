namespace BrainAI.Tests
{
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
            var result = graph.obstacles;
            Assert.AreEqual(4, result[0].points.Count);
            Assert.AreEqual(new Point(2, 2), result[0].points[0]);
            Assert.AreEqual(new Point(3, 2), result[0].points[1]);
            Assert.AreEqual(new Point(3, 3), result[0].points[2]);
            Assert.AreEqual(new Point(2, 3), result[0].points[3]);
        }

        [Test]
        public void BuildGraph_TwoPointsUp()
        {
            var grid = new GridGraph(5, 5);
            grid.Walls.Add(new Point(2, 2));
            grid.Walls.Add(new Point(2, 3));
            var graph = new StrightEdgeGraph();
            GridToStrightEdgeConverter.Default.BuildGraph(grid, graph);
            var result = graph.obstacles;
            Assert.AreEqual(4, result[0].points.Count);
            Assert.AreEqual(new Point(2, 2), result[0].points[0]);
            Assert.AreEqual(new Point(3, 2), result[0].points[1]);
            Assert.AreEqual(new Point(3, 4), result[0].points[2]);
            Assert.AreEqual(new Point(2, 4), result[0].points[3]);
        }

        [Test]
        public void BuildGraph_TwoPointsDown()
        {
            var grid = new GridGraph(5, 5);
            grid.Walls.Add(new Point(2, 2));
            grid.Walls.Add(new Point(2, 1));
            var graph = new StrightEdgeGraph();
            GridToStrightEdgeConverter.Default.BuildGraph(grid, graph);
            var result = graph.obstacles;
            Assert.AreEqual(4, result[0].points.Count);
            Assert.AreEqual(new Point(2, 1), result[0].points[0]);
            Assert.AreEqual(new Point(3, 1), result[0].points[1]);
            Assert.AreEqual(new Point(3, 3), result[0].points[2]);
            Assert.AreEqual(new Point(2, 3), result[0].points[3]);
        }

        [Test]
        public void BuildGraph_TwoPointsRight()
        {
            var grid = new GridGraph(5, 5);
            grid.Walls.Add(new Point(2, 2));
            grid.Walls.Add(new Point(3, 2));
            var graph = new StrightEdgeGraph();
            GridToStrightEdgeConverter.Default.BuildGraph(grid, graph);
            var result = graph.obstacles;
            Assert.AreEqual(4, result[0].points.Count);
            Assert.AreEqual(new Point(2, 2), result[0].points[0]);
            Assert.AreEqual(new Point(4, 2), result[0].points[1]);
            Assert.AreEqual(new Point(4, 3), result[0].points[2]);
            Assert.AreEqual(new Point(2, 3), result[0].points[3]);
        }

        [Test]
        public void BuildGraph_TwoPointsLeft()
        {
            var grid = new GridGraph(5, 5);
            grid.Walls.Add(new Point(2, 2));
            grid.Walls.Add(new Point(1, 2));
            var graph = new StrightEdgeGraph();
            GridToStrightEdgeConverter.Default.BuildGraph(grid, graph);
            var result = graph.obstacles;
            Assert.AreEqual(4, result[0].points.Count);
            Assert.AreEqual(new Point(3, 2), result[0].points[0]);
            Assert.AreEqual(new Point(3, 3), result[0].points[1]);
            Assert.AreEqual(new Point(1, 3), result[0].points[2]);
            Assert.AreEqual(new Point(1, 2), result[0].points[3]);
        }

        [Test]
        public void BuildGraph_IndependentPoints()
        {
            var grid = new GridGraph(5, 5);
            grid.Walls.Add(new Point(2, 2));
            grid.Walls.Add(new Point(4, 2));
            var graph = new StrightEdgeGraph();
            GridToStrightEdgeConverter.Default.BuildGraph(grid, graph);
            var result = graph.obstacles;
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(4, result[0].points.Count);
            Assert.AreEqual(new Point(4, 2), result[0].points[0]);
            Assert.AreEqual(new Point(5, 2), result[0].points[1]);
            Assert.AreEqual(new Point(5, 3), result[0].points[2]);
            Assert.AreEqual(new Point(4, 3), result[0].points[3]);
            Assert.AreEqual(4, result[1].points.Count);
            Assert.AreEqual(new Point(2, 2), result[1].points[0]);
            Assert.AreEqual(new Point(3, 2), result[1].points[1]);
            Assert.AreEqual(new Point(3, 3), result[1].points[2]);
            Assert.AreEqual(new Point(2, 3), result[1].points[3]);
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
            var result = graph.obstacles;
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(4, result[0].points.Count);
            Assert.AreEqual(new Point(2, 2), result[0].points[0]);
            Assert.AreEqual(new Point(4, 2), result[0].points[1]);
            Assert.AreEqual(new Point(4, 4), result[0].points[2]);
            Assert.AreEqual(new Point(2, 4), result[0].points[3]);
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
            var result = graph.obstacles;
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(4, result[0].points.Count);
            Assert.AreEqual(new Point(20, 20), result[0].points[0]);
            Assert.AreEqual(new Point(40, 20), result[0].points[1]);
            Assert.AreEqual(new Point(40, 40), result[0].points[2]);
            Assert.AreEqual(new Point(20, 40), result[0].points[3]);
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
            var result = graph.obstacles;
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(12, result[0].points.Count);
            Assert.AreEqual(new Point(2, 2), result[0].points[0]);
            Assert.AreEqual(new Point(4, 2), result[0].points[1]);
            Assert.AreEqual(new Point(4, 3), result[0].points[2]);
            Assert.AreEqual(new Point(5, 3), result[0].points[3]);
            Assert.AreEqual(new Point(5, 5), result[0].points[4]);
            Assert.AreEqual(new Point(4, 5), result[0].points[5]);
            Assert.AreEqual(new Point(4, 8), result[0].points[6]);
            Assert.AreEqual(new Point(2, 8), result[0].points[7]);
            Assert.AreEqual(new Point(2, 7), result[0].points[8]);
            Assert.AreEqual(new Point(3, 7), result[0].points[9]);
            Assert.AreEqual(new Point(3, 6), result[0].points[10]);
            Assert.AreEqual(new Point(2, 6), result[0].points[11]);
        }
    }
}
