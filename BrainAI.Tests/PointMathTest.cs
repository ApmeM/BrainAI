namespace BrainAI.Tests
{
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
        public void IsDirectionInsidePolygon_InsidePoligon()
        {
            var result = PointMath.IsDirectionInsidePolygon(new Point(0, 0), new Point(2, 2), new Point(0, 1), new Point(2, 0), false);

            Assert.AreEqual(true, result);
        }

        [Test]
        public void IsDirectionInsidePolygon_InsidePoligonConcave1()
        {
            var result = PointMath.IsDirectionInsidePolygon(new Point(30, 10), new Point(15, 15), new Point(40, 10), new Point(30, 20), true);
            Assert.AreEqual(true, result);
        }

        [Test]
        public void IsDirectionInsidePolygon_InsidePoligonConcave2()
        {
            var result = PointMath.IsDirectionInsidePolygon(new Point(40, 10), new Point(15, 15), new Point(40, 30), new Point(30, 10), true);
            Assert.AreEqual(false, result);
        }

        [Test]
        public void IsDirectionInsidePolygon_OutsidePoligon()
        {
            var result = PointMath.IsDirectionInsidePolygon(new Point(0, 0), new Point(-2, -2), new Point(0, 1), new Point(0, 0), false);

            Assert.AreEqual(false, result);
        }

        [Test]
        public void IsDirectionInsidePolygon_InsideRect()
        {
            var result = PointMath.IsDirectionInsidePolygon(new Point(0, 1), new Point(2, 201), new Point(0, 201), new Point(2, 1), false);

            Assert.AreEqual(true, result);
        }

        [Test]
        public void CompareVectorsTest_EqualZeroVectors()
        {
            Assert.AreEqual(0, PointMath.CompareVectors(new Point(15, 15), new Point(15, 15), new Point(15, 15)));
            Assert.AreEqual(1, PointMath.CompareVectors(new Point(15, 15), new Point(15, 15), new Point(-15, 15)));
            Assert.AreEqual(-1, PointMath.CompareVectors(new Point(-15, 15), new Point(15, 15), new Point(15, 15)));
        }

        [Test]
        public void CompareVectorsTest_ZeroY_CompareDirectionOfX()
        {
            Assert.AreEqual(-1, PointMath.CompareVectors(new Point(-15, 15), new Point(15, 15), new Point(20, 15)));
            Assert.AreEqual(1, PointMath.CompareVectors(new Point(20, 15), new Point(15, 15), new Point(-15, 15)));
            Assert.AreEqual(0, PointMath.CompareVectors(new Point(-15, 15), new Point(15, 15), new Point(-10, 15)));
       }

        [Test]
        public void CompareVectorsTest_VectorsInDifferentHalfSphere_TopIsFirst()
        {
            Assert.AreEqual(-1, PointMath.CompareVectors(new Point(-16, -40), new Point(15, 15), new Point(17, 40)));
            Assert.AreEqual(1, PointMath.CompareVectors(new Point(-19, 20), new Point(15, 15), new Point(18, -20)));
        }

        [Test]
        public void CompareVectorsTest_VectorsInSameDirection_Equal()
        {
            Assert.AreEqual(0, PointMath.CompareVectors(new Point(10, 0), new Point(0, 0), new Point(9, 0)));
            Assert.AreEqual(0, PointMath.CompareVectors(new Point(-10, -10), new Point(0, 0), new Point(-12, -12)));
            Assert.AreEqual(0, PointMath.CompareVectors(new Point(-15, 15), new Point(15, 15), new Point(-10, 15)));
            Assert.AreEqual(0, PointMath.CompareVectors(new Point(20, 15), new Point(15, 15), new Point(25, 15)));
            Assert.AreEqual(0, PointMath.CompareVectors(new Point(15, -15), new Point(15, 15), new Point(15, -10)));
            Assert.AreEqual(0, PointMath.CompareVectors(new Point(15, 20), new Point(15, 15), new Point(15, 25)));
        }

        [Test]
        public void CompareVectorsTest_VectorsTopHalfSphere_RightIsFirst()
        {
            Assert.AreEqual(1, PointMath.CompareVectors(new Point(10, 0), new Point(0, 0), new Point(11, 1)));
            Assert.AreEqual(-1, PointMath.CompareVectors(new Point(2, 3), new Point(0, 0), new Point(1, 1)));
        }

        [Test]
        public void CompareVectorsTest_VectorsBottomHalfSphere_LeftIsFirst()
        {
            Assert.AreEqual(1, PointMath.CompareVectors(new Point(10, -50), new Point(0, 0), new Point(11, -20)));
            Assert.AreEqual(-1, PointMath.CompareVectors(new Point(10, -50), new Point(0, 0), new Point(-11, -20)));
        }

        [Test]
        public void CompareVectorsTest_OneVectorDirectedToZero()
        {
            Assert.AreEqual(1, PointMath.CompareVectors(new Point(-2, 0), new Point(0, 0), new Point(10, -18)));
            Assert.AreEqual(-1, PointMath.CompareVectors(new Point(10, -18), new Point(0, 0), new Point(-2, 0)));
        }
    }
}
