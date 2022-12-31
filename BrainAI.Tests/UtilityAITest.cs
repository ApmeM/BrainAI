namespace BrainAI.Tests
{
    using BrainAI.AI.UtilityAI;

    using NUnit.Framework;

    [TestFixture]
    public class UtilityAITest
    {
        private class Context
        {
            public int i = 0;
            public int enter1;
            public int exit1;
            public int enter2;
            public int exit2;
        }

        [Test]
        public void UtilityAI_Tick1_EnterAndExecuteFirstAction()
        {
            var context = new Context();

            var reasoner = new LowestScoreReasoner<Context>();
            reasoner.Add(new ActionAppraisal<Context>(a => a.i), new ActionAction<Context>(c => c.enter1++, c => c.i++, c => c.exit1++));
            reasoner.Add(new ActionAppraisal<Context>(a => 2 - a.i), new ActionAction<Context>(c => c.enter2++, c => { }, c => c.exit2++));
            var target = new UtilityAI<Context>(context, reasoner);

            target.Tick();

            Assert.AreEqual(1, context.enter1);
            Assert.AreEqual(0, context.exit1);
            Assert.AreEqual(0, context.enter2);
            Assert.AreEqual(0, context.exit2);
            Assert.AreEqual(1, context.i);
        }

        [Test]
        public void UtilityAI_Tick2_ExitFirstActionAndEnterAndExecuteSecondAction()
        {
            var context = new Context
            {
                i = 1
            };

            var reasoner = new LowestScoreReasoner<Context>();
            reasoner.Add(new ActionAppraisal<Context>(a => a.i), new ActionAction<Context>(c => c.enter1++, c => c.i++, c => c.exit1++));
            reasoner.Add(new ActionAppraisal<Context>(a => 2 - a.i), new ActionAction<Context>(c => c.enter2++, c => { }, c => c.exit2++));
            var target = new UtilityAI<Context>(context, reasoner);

            target.Tick();
            target.Tick();
            target.Tick();

            Assert.AreEqual(1, context.enter1);
            Assert.AreEqual(1, context.exit1);
            Assert.AreEqual(1, context.enter2);
            Assert.AreEqual(0, context.exit2);
            Assert.AreEqual(2, context.i);
        }

        [Test]
        [TestCase(0,0,0)]
        [TestCase(0,1,1)]
        [TestCase(1,0,1)]
        [TestCase(1,1,1)]
        public void FirstAfterThresholdAppraisal_ActAsBoolOrOperator(float v1, float v2, float res)
        {
            var target = new MaxAppraisal<Context>(
                new FixedAppraisal<Context>(v1),
                new FixedAppraisal<Context>(v2)
            );

            var score = target.GetScore(new Context());

            Assert.AreEqual(res, score);
        }

        [Test]
        [TestCase(0,0,0)]
        [TestCase(0,1,0)]
        [TestCase(1,0,0)]
        [TestCase(1,1,1)]
        public void FirstOrNothingAppraisal_ActAsBoolAndOperation(float v1, float v2, float res)
        {
            var target = new MultAppraisal<Context>(
                new FixedAppraisal<Context>(v1),
                new FixedAppraisal<Context>(v2)
            );

            var score = target.GetScore(new Context());

            Assert.AreEqual(res, score);
        }
    }
}
