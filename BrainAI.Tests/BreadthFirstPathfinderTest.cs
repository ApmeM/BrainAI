using BrainAI.Pathfinding;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace BrainAI.Tests
{
    [TestFixture]
    public class BreadthFirstPathfinderTest
    {
        private UnweightedGridGraph graph;
        private ICoveragePathfinder<Point> pathfinder;

        [SetUp]
        public void Setup()
        {
            this.graph = new UnweightedGridGraph(10, 10);
            this.pathfinder = new BreadthFirstPathfinder<Point>(graph);
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
            var target = new UnweightedGridGraph(10, 10, true);
            target.Walls.Add(new Point(1, 2));
            target.Walls.Add(new Point(2, 1));
            target.Walls.Add(new Point(1, 0));
            target.Walls.Add(new Point(0, 0));
            var result = new BreadthFirstPathfinder<Point>(target).Search(new Point(1, 1), new Point(2, 2));
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual(new Point(1, 1), result[0]);
            Assert.AreEqual(new Point(2, 2), result[1]);
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
            var result = pathfinder.Search(new Point(1, 1), new Point(2, 2));
            Assert.AreEqual(3, result.Count());
            Assert.AreEqual(new Point(1, 1), result[0]);
            Assert.AreEqual(new Point(2, 1), result[1]);
            Assert.AreEqual(new Point(2, 2), result[2]);
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
            var result = pathfinder.Search(new Point(1, 1), new Point(2, 2));
            Assert.AreEqual(7, result.Count());
            Assert.AreEqual(new Point(1, 1), result[0]);
            Assert.AreEqual(new Point(0, 1), result[1]);
            Assert.AreEqual(new Point(0, 2), result[2]);
            Assert.AreEqual(new Point(0, 3), result[3]);
            Assert.AreEqual(new Point(1, 3), result[4]);
            Assert.AreEqual(new Point(2, 3), result[5]);
            Assert.AreEqual(new Point(2, 2), result[6]);
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
            var result = pathfinder.Search(new Point(1, 1), new Point(2, 2));
            Assert.AreEqual(null, result);
        }

        [Test]
        public void Search_Distance_DictionaryContainsOnlyFound()
        {
            /*
             _#__
             #0#_
             ____
             ____
            */
            graph.Walls.Add(new Point(2, 1));
            graph.Walls.Add(new Point(1, 0));
            graph.Walls.Add(new Point(0, 1));
            pathfinder.Search(new Point(1, 1), 2);
            Assert.AreEqual(5, pathfinder.VisitedNodes.Count());
            Assert.IsTrue(pathfinder.VisitedNodes.ContainsKey(new Point(1, 1)));
            Assert.IsTrue(pathfinder.VisitedNodes.ContainsKey(new Point(1, 2)));
            Assert.IsTrue(pathfinder.VisitedNodes.ContainsKey(new Point(1, 3)));
            Assert.IsTrue(pathfinder.VisitedNodes.ContainsKey(new Point(2, 2)));
            Assert.IsTrue(pathfinder.VisitedNodes.ContainsKey(new Point(0, 2)));
        }

        [Test]
        public void Search_DistanceNoWay_ContainsOnlyReachable()
        {
            /*
             _#__
             #0#_
             #_#_
             _#__
            */
            graph.Walls.Add(new Point(2, 1));
            graph.Walls.Add(new Point(2, 2));
            graph.Walls.Add(new Point(0, 2));
            graph.Walls.Add(new Point(1, 3));
            graph.Walls.Add(new Point(1, 0));
            graph.Walls.Add(new Point(0, 1));
            pathfinder.Search(new Point(1, 1), 2);
            Assert.AreEqual(2, pathfinder.VisitedNodes.Count());
            Assert.IsTrue(pathfinder.VisitedNodes.ContainsKey(new Point(1, 1)));
            Assert.IsTrue(pathfinder.VisitedNodes.ContainsKey(new Point(1, 2)));
        }

        [Test]
        public void Search_DistanceZero_ContainsOnlyOneItem()
        {
            /*
             _#__
             _0__
             ____
             ____
            */
            graph.Walls.Add(new Point(1, 0));
            pathfinder.Search(new Point(1, 1), 0);
            Assert.AreEqual(1, pathfinder.VisitedNodes.Count());
            Assert.IsTrue(pathfinder.VisitedNodes.ContainsKey(new Point(1, 1)));
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
            var result = pathfinder.Search(new Point(1, 1), new HashSet<Point> { new Point(3, 2), new Point(2, 2) });
            Assert.AreEqual(3, result.Count());
            Assert.AreEqual(new Point(1, 1), result[0]);
            Assert.AreEqual(new Point(2, 1), result[1]);
            Assert.AreEqual(new Point(2, 2), result[2]);
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
            var result = pathfinder.Search(new Point(1, 1), new HashSet<Point> { new Point(2, 3), new Point(2, 2) });
            Assert.AreEqual(3, result.Count());
            var secondResult = pathfinder.ContinueSearch();
            Assert.AreEqual(4, secondResult.Count());

            Assert.AreEqual(new Point(1, 1), secondResult[0]);
            Assert.AreEqual(new Point(2, 1), secondResult[1]);
            Assert.AreEqual(new Point(2, 2), secondResult[2]);
            Assert.AreEqual(new Point(2, 3), secondResult[3]);
        }
    }
}
