using BrainAI.Pathfinding;
using Microsoft.VisualBasic;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace BrainAI.Tests
{
    [TestFixture]
    public class WeightedPathfinderTest
    {
        private GridGraph graph;
        private ICoveragePathfinder<Point> pathfinder;

        [SetUp]
        public void Setup()
        {
            this.graph = new GridGraph(10, 10);
            this.pathfinder = new WeightedPathfinder<Point>(graph);
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
            this.pathfinder = new WeightedPathfinder<Point>(graph);

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
    }
}
