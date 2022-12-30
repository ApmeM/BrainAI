namespace BrainAI.Tests
{
    using BrainAI.InfluenceMap;
    using BrainAI.Pathfinding;
    using NUnit.Framework;

    [TestFixture]
    public class MatrixInfluenceMapTest
    {
        [Test]
        public void FindForceDirection_SinglePointOfAttraction_DirectToIt()
        {
            var target = new MatrixInfluenceMap(5, 5);
            target.AddCharge("default", new PointChargeOrigin(new Point(0, 0)), DefaultFadings.NoDistanceFading, 10);

            Assert.AreEqual(10, target.GetChargeAtPoint(new Point(3, 3)));
        }

        [Test]
        public void FindForceDirection_MultiplePointsOfAttraction_SumOfDirections()
        {
            var target = new MatrixInfluenceMap(5, 5);
            target.AddCharge("default", new PointChargeOrigin(new Point(0, 0)), DefaultFadings.LinearDistanceFading, 10);
            target.AddCharge("default", new PointChargeOrigin(new Point(4, 4)), DefaultFadings.LinearDistanceFading, -10);

            Assert.AreEqual(0, target.GetChargeAtPoint(new Point(0, 4)));
        }
    }
}
