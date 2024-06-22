using BrainAI.Pathfinding;
using NUnit.Framework;
using System.Collections.Generic;

namespace BrainAI.Tests
{
    [TestFixture]
    public class AStarPathfinderTest
    {
        private GridGraph graph;
        private AStarPathfinder<Point> pathfinder;

        [SetUp]
        public void Setup()
        {
            this.graph = new GridGraph(10, 10);
            this.pathfinder = new AStarPathfinder<Point>(graph);
        }

        [Test]
        public void Search_AllowDiagonal_PathFound()
        {
            /*
             ##__
             _0#_
             _#1_
             ____
            */
            this.graph = new GridGraph(10, 10, true);
            this.pathfinder = new AStarPathfinder<Point>(graph);

            graph.Walls.Add(new Point(1, 2));
            graph.Walls.Add(new Point(2, 1));
            graph.Walls.Add(new Point(1, 0));
            graph.Walls.Add(new Point(0, 0));
            pathfinder.Search(new Point(1, 1), new Point(2, 2));
            CollectionAssert.AreEqual(new List<Point> {
                new Point(1, 1),
                new Point(2, 2)
            }, pathfinder.ResultPath);
        }

        [Test]
        public void Search_ForwardPath_PathFound()
        {
            /*
             ____
             _01_
             _#2_
            */
            graph.Walls.Add(new Point(1, 2));

            pathfinder.Search(new Point(1, 1), new Point(2, 2));
            CollectionAssert.AreEqual(new List<Point> {
                new Point(1, 1),
                new Point(2, 1),
                new Point(2, 2)
            }, pathfinder.ResultPath);
        }

        [Test]
        public void Search_BackwardPath_PathFound()
        {
            /*
             ##__
             10#_
             2#6_
             345_
            */
            graph.Walls.Add(new Point(1, 2));
            graph.Walls.Add(new Point(2, 1));
            graph.Walls.Add(new Point(1, 0));
            graph.Walls.Add(new Point(0, 0));

            pathfinder.Search(new Point(1, 1), new Point(2, 2));
            CollectionAssert.AreEqual(new List<Point> {
                new Point(1, 1),
                new Point(0, 1),
                new Point(0, 2),
                new Point(0, 3),
                new Point(1, 3),
                new Point(2, 3),
                new Point(2, 2)
            }, pathfinder.ResultPath);
        }

        [Test]
        public void Search_NoWay_PathNull()
        {
            /*
             _#__
             #0#_
             _#x_
             ____
            */
            graph.Walls.Add(new Point(1, 2));
            graph.Walls.Add(new Point(2, 1));
            graph.Walls.Add(new Point(1, 0));
            graph.Walls.Add(new Point(0, 1));

            pathfinder.Search(new Point(1, 1), new Point(2, 2));

            Assert.IsEmpty(pathfinder.ResultPath);
        }

        [Test]
        public void Search_MultiGoals_PathFound()
        {
            /*
             ____
             _01_
             _#2_
            */
            graph.Walls.Add(new Point(1, 2));
            pathfinder.Search(new Point(1, 1), new HashSet<Point> { new Point(3, 2), new Point(2, 2) });
            CollectionAssert.AreEqual(new List<Point> {
                new Point(1, 1),
                new Point(2, 1),
                new Point(2, 2)
            }, pathfinder.ResultPath);
        }

        [Test]
        public void ContinueSearch_MultiGoals_PathFound()
        {
            /*
             ____
             _01_
             _#2_
             __3_
            */
            graph.Walls.Add(new Point(1, 2));
            pathfinder.Search(new Point(1, 1), new HashSet<Point> { new Point(2, 3), new Point(2, 2) });
            CollectionAssert.AreEqual(new List<Point> {
                new Point(1, 1),
                new Point(2, 1),
                new Point(2, 2)
            }, pathfinder.ResultPath);
            pathfinder.ContinueSearch();
            CollectionAssert.AreEqual(new List<Point> {
                new Point(1, 1),
                new Point(2, 1),
                new Point(2, 2),
                new Point(2, 3)
            }, pathfinder.ResultPath);
        }

        [Test]
        public void Search_TwiceWithSuccess_PathCleared()
        {
            /*
             ____
             _01_
             _#2_
             __3_
            */
            graph.Walls.Add(new Point(1, 2));
            pathfinder.Search(new Point(1, 1), new Point(2, 3));
            CollectionAssert.AreEqual(new List<Point> {
                new Point(1, 1),
                new Point(2, 1),
                new Point(2, 2),
                new Point(2, 3)
            }, pathfinder.ResultPath);
            pathfinder.Search(new Point(1, 1), new Point(2, 3));
            CollectionAssert.AreEqual(new List<Point> {
                new Point(1, 1),
                new Point(2, 1),
                new Point(2, 2),
                new Point(2, 3)
            }, pathfinder.ResultPath);
        }
        [Test]
        public void Search_TwiceWithFail_PathCleared()
        {
            /*
             ###_
             #012
             ###_
             ____
            */
            graph.Walls.Add(new Point(0, 2));
            graph.Walls.Add(new Point(0, 1));
            graph.Walls.Add(new Point(0, 0));
            graph.Walls.Add(new Point(1, 0));
            graph.Walls.Add(new Point(2, 0));
            graph.Walls.Add(new Point(2, 1));
            graph.Walls.Add(new Point(2, 2));
            pathfinder.Search(new Point(1, 1), new Point(1, 3));
            CollectionAssert.AreEqual(new List<Point> {
                new Point(1, 1),
                new Point(1, 2),
                new Point(1, 3)
            }, pathfinder.ResultPath);

            
            /*
             ###_
             #012
             ###_
             ____
            */
            graph.Walls.Add(new Point(1, 2));
            pathfinder.Search(new Point(1, 1), new Point(1, 3));
            CollectionAssert.IsEmpty(pathfinder.ResultPath);
        }

        [Test]
        public void MoveFromBlock()
        {
            /*
             ____
             _01_
             _#2_
             __3_
            */
            graph.Walls.Add(new Point(1, 2));
            pathfinder.Search(new Point(1, 2), new Point(2, 3));
            CollectionAssert.AreEqual(new List<Point> {
                new Point(1, 2),
                new Point(2, 2),
                new Point(2, 3)
            }, pathfinder.ResultPath);
        }
    }
}
