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

            graph.AddObstacle(
                new List<Point>{
                        new Point( 200, 300),
                        new Point(1000, 300),
                        new Point(1000, 500),
                        new Point( 200, 500),
                    });
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
                graph.AddObstacle(
                    new List<Point>{
                        new Point(j*4 + 0 + 0, j*4 + 0 + 1),
                        new Point(j*4 + 2 + 0, j*4 + 0 + 1),
                        new Point(j*4 + 2 + 0, j*4 + 200 + 1),
                        new Point(j*4 + 0 + 0, j*4 + 200 + 1),
                    });
                graph.AddObstacle(
                    new List<Point>{
                        new Point(j*4 + 0 + 3, j*4 + 0 + 2),
                        new Point(j*4 + 200 + 3, j*4 + 0 + 2),
                        new Point(j*4 + 200 + 3, j*4 + 2 + 2),
                        new Point(j*4 + 0 + 3, j*4 + 2 + 2),
                    });
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
    }
}
