namespace BrainAI.Tests
{
    using System;
    using System.Collections.Generic;
    using BrainAI.Pathfinding;

    using NUnit.Framework;

    [TestFixture]
    public class PointMathTest
    {
        [Test]
        public void DoubledTriangleSquareBy3Dots_CW()
        {
            var result = PointMath.DoubledTriangleSquareBy3Dots(new Point(1, 0), new Point(0, 0), new Point(0, 1));
            Assert.AreEqual(-1, result);
        }

        [Test]
        public void DoubledTriangleSquareBy3Dots_CCW()
        {
            var result = PointMath.DoubledTriangleSquareBy3Dots(new Point(0, 1), new Point(0, 0), new Point(1, 0));
            Assert.AreEqual(1, result);
        }

        [Test]
        public void DoubledTriangleSquareBy3Dots_DotPodPerpendicular()
        {
            var result1 = PointMath.DoubledTriangleSquareBy3Dots(new Point(0, 1), new Point(0, 0), new Point(1, 0));
            var result2 = PointMath.DotProdFor2VecotrsWithOneOrigin(new Point(0, 1), new Point(0, 0), new Point(0, 1));
            Assert.AreEqual(result1, result2);
        }

        [Test]
        public void PointToLineDistSq()
        {
            var result = PointMath.PointToLineDistSq(new Point(-1, -1), new Point(0, 1), new Point(1, 0));
            Assert.AreEqual(4.5, result);
        }

        [Test]
        public void CalcCenterOfPolygon_CCW()
        {
            var result = PointMath.CalcCenterOfPolygon(new List<Point> { new Point(0, -1), new Point(0, 1), new Point(2, 0) });
            Assert.AreEqual(false, result.Item2);
            Assert.AreEqual(new Point(1, 0), result.Item1);
        }

        [Test]
        public void PointWithinPolygon_Inside()
        {
            var result = PointMath.PointWithinPolygon(new List<Point> { new Point(0, -1), new Point(0, 1), new Point(2, 0) }, new Point(1, 0));
            Assert.AreEqual(true, result);
        }

        [Test]
        public void PointWithinPolygon_Outside()
        {
            var result = PointMath.PointWithinPolygon(new List<Point> { new Point(0, -1), new Point(0, 1), new Point(2, 0) }, new Point(10, 0));
            Assert.AreEqual(false, result);
        }

        [Test]
        public void PointWithinPolygon_Vertex()
        {
            var result = PointMath.PointWithinPolygon(new List<Point> { new Point(0, -1), new Point(0, 1), new Point(2, 0) }, new Point(2, 0));
            Assert.AreEqual(true, result);
        }

        [Test]
        public void PointWithinPolygon_Edge()
        {
            var result = PointMath.PointWithinPolygon(new List<Point> { new Point(0, 0), new Point(0, 1), new Point(2, 0) }, new Point(1, 0));
            Assert.AreEqual(true, result);
        }

        [Test]
        public void SegmentIntersectsPolygon_IntersectsEdge()
        {
            var result = PointMath.SegmentIntersectsPolygon(
                new List<Point> { new Point(0, 0), new Point(0, 1), new Point(2, 0) },
                new Point(1, -1), new Point(1, 1),
                true
                );
            Assert.AreEqual(true, result);
        }

        [Test]
        public void SegmentIntersectsPolygon_NotIntersectsEdge()
        {
            var result = PointMath.SegmentIntersectsPolygon(
                new List<Point> { new Point(0, 0), new Point(0, 1), new Point(2, 0) },
                new Point(1, -1), new Point(1, -2),
                true
                );
            Assert.AreEqual(false, result);
        }

        [Test]
        public void SegmentIntersectsPolygon_IntersectsWithVertex()
        {
            var result = PointMath.SegmentIntersectsPolygon(
                new List<Point> { new Point(0, 0), new Point(0, 1), new Point(2, 0) },
                new Point(2, -1), new Point(2, 2),
                false
                );
            Assert.AreEqual(true, result);
        }

        [Test]
        public void SegmentIntersectsPolygon_IntersectsWithVertexButNoNeed()
        {
            var result = PointMath.SegmentIntersectsPolygon(
                new List<Point> { new Point(0, 0), new Point(0, 1), new Point(2, 0) },
                new Point(2, -1), new Point(2, 2),
                true
                );
            Assert.AreEqual(true, result);
        }

        [Test]
        public void SegmentIntersectCircle_NotIntersecting()
        {
            var result = PointMath.SegmentIntersectCircle(
                new Point(2, -1), new Point(2, 2),
                new Point(0, 0), 3);
            Assert.AreEqual(false, result);
        }

        [Test]
        public void SegmentIntersectCircle_IntersectingOnABorder()
        {
            var result = PointMath.SegmentIntersectCircle(
                new Point(2, -1), new Point(2, 2),
                new Point(0, 0), 4);
            Assert.AreEqual(true, result);
        }

        [Test]
        public void SegmentIntersectCircle_FullyIntersecting()
        {
            var result = PointMath.SegmentIntersectCircle(
                new Point(2, -1), new Point(2, 2),
                new Point(0, 0), 5);
            Assert.AreEqual(true, result);
        }

        [Test]
        public void SegmentIntersectCircle_DirectedButNotIntersected()
        {
            var result = PointMath.SegmentIntersectCircle(
                new Point(0, -10), new Point(0, -3),
                new Point(0, 0), 4);
            Assert.AreEqual(false, result);
        }

        [Test]
        public void SegmentIntersectCircle_DirectedAndTouched()
        {
            var result = PointMath.SegmentIntersectCircle(
                new Point(0, -10), new Point(0, -2),
                new Point(0, 0), 4);
            Assert.AreEqual(true, result);
        }

        [Test]
        public void IsDirectionInsidePolygon_InsidePoligon()
        {
            var result = PointMath.IsDirectionInsidePolygon(new Point(0, 0), new Point(2, 2), new Point(0, 1), new Point(2, 0));

            Assert.AreEqual(true, result);
        }

        [Test]
        public void IsDirectionInsidePolygon_OutsidePoligon()
        {
            var result = PointMath.IsDirectionInsidePolygon(new Point(0, 0), new Point(-2, -2), new Point(0, 1), new Point(0, 0));

            Assert.AreEqual(false, result);
        }

        [Test]
        public void IsDirectionInsidePolygon_InsideRect()
        {
            var result = PointMath.IsDirectionInsidePolygon(new Point(0, 1), new Point(2, 201), new Point(0, 201), new Point(2, 1));

            Assert.AreEqual(true, result);
        }
    }
}
