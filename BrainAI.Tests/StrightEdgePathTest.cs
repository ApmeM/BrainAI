using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace BrainAI.Pathfinding
{
    [TestFixture]
    public class PathTest
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

            graph.AddObstacle(BuildObstacle(100, 100));
            graph.AddObstacle(BuildObstacle(400, 400));

            var start = new Point(0, 0);
            var end = new Point(500, 500);
            var pathData = new AStarPathfinder<Point>(graph).Search(start, end);
            Assert.AreEqual(5, pathData.Count);
        }


        [Test]
        public void WithObstacles()
        {
            var graph = new StrightEdgeGraph();
            for (int i = 1; i < 5; i++)
            {
                for (int j = 1; j < 5; j++)
                {
                    graph.AddObstacle(BuildObstacle(i * 100, j * 100));
                }
            }

            var start = new Point(0, 0);
            var end = new Point(500, 500);
            var pathData = new AStarPathfinder<Point>(graph).Search(start, end);
            Assert.AreEqual(4, pathData.Count);
        }

        [Test]
        public void WithPathThroughAllObstacles()
        {
            var graph = new StrightEdgeGraph();

            for (var i = 0; i < 100 / 2; i++)
            {
                var j = i / 2;
                switch (i % 2)
                {
                    case 0:
                        graph.AddObstacle(
                            new List<Point>{
                                new Point(j*4 + 0 + 0, j*4 + 0 + 1),
                                new Point(j*4 + 2 + 0, j*4 + 0 + 1),
                                new Point(j*4 + 2 + 0, j*4 + 100 + 1),
                                new Point(j*4 + 0 + 0, j*4 + 100 + 1),
                            });
                        break;
                    case 1:
                        graph.AddObstacle(
                            new List<Point>{
                                new Point(j*4 + 0 + 3, j*4 + 0 + 2),
                                new Point(j*4 + 2 + 3, j*4 + 0 + 2),
                                new Point(j*4 + 2 + 3, j*4 + 2 + 2),
                                new Point(j*4 + 0 + 3, j*4 + 2 + 2),
                            });
                        break;
                }
            }

            var start = new Point(0, 0);
            var end = new Point(110, 110);
            var pathData = new AStarPathfinder<Point>(graph).Search(start, end);
            Assert.AreEqual(new Point(0,0), pathData[0]);
            Assert.AreEqual(new Point(5,2), pathData[1]);
            Assert.AreEqual(new Point(101,98), pathData[2]);
            Assert.AreEqual(new Point(110,110), pathData[3]);
        }

        private List<Point> BuildObstacle(int x, int y)
        {
            var pointList = new List<Point>();
            double currentAngle = 0;
            for (int k = 0; k < 6; k++)
            {
                pointList.Add(new Point((int)(50 * Math.Cos(currentAngle) + x), (int)(50 * Math.Sin(currentAngle) + y)));
                currentAngle += Math.PI * 2f / 12;
                pointList.Add(new Point((int)(20 * Math.Cos(currentAngle) + x), (int)(20 * Math.Sin(currentAngle) + y)));
                currentAngle += Math.PI * 2f / 12;
            }
            return pointList;
        }
    }
}
