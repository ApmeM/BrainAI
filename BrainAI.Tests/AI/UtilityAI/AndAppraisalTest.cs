namespace BrainAI.Tests
{
    using BrainAI.AI.UtilityAI;

    using NUnit.Framework;

    [TestFixture]
    public class AndAppraisalTest
    {
        private class Context
        {
        }

        [Test]
        public void GetScore_NoAppraisals_False()
        {
            var context = new Context();

            var and = new AndAppraisal<Context>();
            var score = and.GetScore(context);

            Assert.AreEqual(0, score);
        }

        [Test]
        public void GetScore_AllAppraisalsHaveValues_True()
        {
            var context = new Context();

            var and = new AndAppraisal<Context>(
                new FixedAppraisal<Context>(1),
                new FixedAppraisal<Context>(-1)
            );
            var score = and.GetScore(context);

            Assert.AreEqual(1, score);
        }

        [Test]
        public void GetScore_NotAllAppraisalsHaveValues_False()
        {
            var context = new Context();

            var and = new AndAppraisal<Context>(
                new FixedAppraisal<Context>(1),
                new FixedAppraisal<Context>(-1),
                new FixedAppraisal<Context>(0)
            );
            var score = and.GetScore(context);

            Assert.AreEqual(0, score);
        }

        [Test]
        public void GetScore_AllAppraisalsHaveNoValues_False()
        {
            var context = new Context();

            var and = new AndAppraisal<Context>(
                new FixedAppraisal<Context>(0),
                new FixedAppraisal<Context>(0),
                new FixedAppraisal<Context>(0)
            );
            var score = and.GetScore(context);

            Assert.AreEqual(0, score);
        }
    }
}
