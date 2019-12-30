using BrainAI.InfluenceMap.Fading;
using BrainAI.Pathfinding;
using BrainAI.Pathfinding.BreadthFirst;
using NUnit.Framework;
using System.Linq;

namespace BrainAI.Tests
{
    [TestFixture]
    public class LinearDistanceFadingTest
    {
        [Test]
        public void GetForce_Faiding1_ChargeReduced()
        {
            var fading = new LinearDistanceFading(1);
            var result = fading.GetForce(new Point(100, 0), 400);
            Assert.AreEqual(300, result.X);
            Assert.AreEqual(0, result.Y);
        }

        [Test]
        public void GetForce_Faiding2_ChargeReducedDuble()
        {
            var fading = new LinearDistanceFading(2);
            var result = fading.GetForce(new Point(100, 0), 400);
            Assert.AreEqual(200, result.X);
            Assert.AreEqual(0, result.Y);
        }

        [Test]
        public void GetForce_CloseToCener_CloseMaxChargeValue()
        {
            var fading = new LinearDistanceFading(2.5);
            var result = fading.GetForce(new Point(1, 0), 400);
            Assert.AreEqual(397, result.X);
            Assert.AreEqual(0, result.Y);
        }
    }
}
