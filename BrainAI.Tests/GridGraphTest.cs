using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace BrainAI.Pathfinding
{
    [TestFixture]
    public class GridGraphTest
    {
        [Test]
        public void GetNeighboursForTopLeftCorner_WidthHeightSet_Two()
        {
            var graph = new GridGraph(10, 10, false);
            var result = new List<Point>();
            graph.GetNeighbors(new Point(0, 0), result);

            CollectionAssert.AreEquivalent(new[] { new Point(1, 0), new Point(0, 1) }, result);
        }

        [Test]
        public void GetNeighboursForOutsideTopLeftCorner_WidthHeightSet_None()
        {
            var graph = new GridGraph(10, 10, false);
            var result = new List<Point>();
            graph.GetNeighbors(new Point(-1, -1), result);

            CollectionAssert.AreEquivalent(new Point[] { }, result);
        }


        [Test]
        public void GetNeighboursForBottomRightCorner_WidthHeightSet_Two()
        {
            var graph = new GridGraph(10, 10, false);
            var result = new List<Point>();
            graph.GetNeighbors(new Point(9, 9), result);

            CollectionAssert.AreEquivalent(new[] { new Point(9, 8), new Point(8, 9) }, result);
        }

        [Test]
        public void GetNeighboursForOutsideBottomRight_WidthHeightSet_None()
        {
            var graph = new GridGraph(10, 10, false);
            var result = new List<Point>();
            graph.GetNeighbors(new Point(10, 10), result);

            CollectionAssert.AreEquivalent(new Point[] { }, result);
        }

        [Test]
        public void GetNeighboursForInside_WidthHeightSet_None()
        {
            var graph = new GridGraph(10, 10, false);
            var result = new List<Point>();
            graph.GetNeighbors(new Point(5, 5), result);

            CollectionAssert.AreEquivalent(new[] { new Point(4, 5), new Point(5, 4), new Point(5, 6), new Point(6, 5) }, result);
        }


        [Test]
        public void GetNeighboursForTopLeftCorner_BordersSet_Two()
        {
            var graph = new GridGraph(-10, -10, 10, 10, false);
            var result = new List<Point>();
            graph.GetNeighbors(new Point(-10, -10), result);

            CollectionAssert.AreEquivalent(new[] { new Point(-10, -9), new Point(-9, -10) }, result);
        }

        [Test]
        public void GetNeighboursForOutsideTopLeftCorner_BordersSet_None()
        {
            var graph = new GridGraph(-10, -10, 10, 10, false);
            var result = new List<Point>();
            graph.GetNeighbors(new Point(-11, -11), result);

            CollectionAssert.AreEquivalent(new Point[] { }, result);
        }


        [Test]
        public void GetNeighboursForBottomRightCorner_BordersSet_Two()
        {
            var graph = new GridGraph(-10, -10, 10, 10, false);
            var result = new List<Point>();
            graph.GetNeighbors(new Point(10, 10), result);

            CollectionAssert.AreEquivalent(new[] { new Point(9, 10), new Point(10, 9) }, result);
        }

        [Test]
        public void GetNeighboursForOutsideBottomRight_BordersSet_None()
        {
            var graph = new GridGraph(-10, -10, 10, 10, false);
            var result = new List<Point>();
            graph.GetNeighbors(new Point(11, 11), result);

            CollectionAssert.AreEquivalent(new Point[] { }, result);
        }

        [Test]
        public void GetNeighboursForInside_BordersSet_None()
        {
            var graph = new GridGraph(-10, -10, 10, 10, false);
            var result = new List<Point>();
            graph.GetNeighbors(new Point(0, 0), result);

            CollectionAssert.AreEquivalent(new[] { new Point(-1, 0), new Point(1, 0), new Point(0, -1), new Point(0, 1) }, result);
        }
    }
}
