namespace BrainAI.Tests
{
    using BrainAI.AI.UtilityAI;

    using NUnit.Framework;

    [TestFixture]
    public class IntentTest
    {
        private class Context
        {
        }

        [Test]
        public void CompositeIntent_Execute_RemovesFinished()
        {
            var context = new Context();
            var i = 2;
            var j = 0;
            var intent = new CompositeIntent<Context>(
                new ActionIntent<Context>(a => j++ >= 0),
                new ActionIntent<Context>(a => i-- == 0),
                new ActionIntent<Context>(a => j++ >= 0)
            );

            intent.Execute(context);
            Assert.AreEqual(1, i);
            Assert.AreEqual(2, j);

            intent.Execute(context);
            Assert.AreEqual(0, i);
            Assert.AreEqual(2, j);
        }
    }
}
