namespace BrainAI.Tests
{
    using BrainAI.InfluenceMap;
    using BrainAI.InfluenceMap.Fading;
    using BrainAI.InfluenceMap.VectorGenerator;
    using BrainAI.Pathfinding;
    using NUnit.Framework;

    [TestFixture]
    public class VectorInfluenceMapTest
    {
        [Test]
        public void FindForceDirection_SinglePointOfAttraction_DirectToIt()
        {
            var target = new VectorInfluenceMap();
            target.AddCharge(
                "default", 
                new PointChargeOrigin(new Point(0, 0)),
                DefaultFadings.NoDistanceFading,
                10);

            var force = target.FindForceDirection(new Point(10, 0));

            Assert.AreEqual(new Point(-10, 0), force);
        }

        [Test]
        public void FindForceDirection_MultiplePointsOfAttraction_SumOfDirections()
        {
            var target = new VectorInfluenceMap();
            target.AddCharge(
                "default", 
                new PointChargeOrigin(new Point(0, 0)),
                DefaultFadings.NoDistanceFading,
                10);

            target.AddCharge(
                "default", 
                new PointChargeOrigin(new Point(10, 10)),
                DefaultFadings.NoDistanceFading,
                10);

            var force = target.FindForceDirection(new Point(10, 0));

            Assert.AreEqual(new Point(-10, 10), force);
        }
    }
}
