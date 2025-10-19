namespace BrainAI.Tests
{
    using BrainAI.AI.UtilityAI;

    using NUnit.Framework;

    [TestFixture]
    public class NotAppraisalTest
    {
        private class Context
        {
        }

        [Test]
        public void GetScore_AppraisalHaveValues_False()
        {
            var context = new Context();

            var and = new NotAppraisal<Context>(
                new FixedAppraisal<Context>(-1)
            );
            var score = and.GetScore(context);

            Assert.AreEqual(0, score);
        }

        [Test]
        public void GetScore_AppraisalHaveNoValues_True()
        {
            var context = new Context();

            var and = new NotAppraisal<Context>(
                new FixedAppraisal<Context>(0)
            );
            var score = and.GetScore(context);

            Assert.AreEqual(1, score);
        }
    }
}
