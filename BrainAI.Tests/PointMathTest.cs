namespace BrainAI.Tests
{
    using BrainAI.Pathfinding;

    using NUnit.Framework;

    [TestFixture]
    public class PointMathTest
    {
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
        public void FindEndPoint_ToCCW()
        {
            Assert.AreEqual((0, new Point(10,30)), PointMath.FindEndPoint(new Point(15, 15), (0, new Point(10, 10)), new Point(0, 10), new Point(10, 30)));
        }
    }
}
