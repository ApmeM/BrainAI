namespace BrainAI.Tests
{
    using BrainAI.Tests.AI;

    using NUnit.Framework;

    [TestFixture]
    public class AITest
    {
        [Test]
        public void Utility_Test()
        {
            var ai = UtilityMiner.BuildAI();
            for (var i = 0; i < 100; i++)
            {
                ai.Tick();
            }
        }

        [Test]
        public void BehaviorTree1_Test()
        {
            var ai = BehaviorTreeMiner.BuildSelfAbortTree();
            for (var i = 0; i < 100; i++)
            {
                ai.Tick();
            }
        }

        [Test]
        public void BehaviorTree2_Test()
        {
            var ai = BehaviorTreeMiner.BuildLowerPriorityAbortTree();
            for (var i = 0; i < 100; i++)
            {
                ai.Tick();
            }
        }

        [Test]
        public void GOAP_Test()
        {
            var ai = GOAPMiner.BuildAI();
            for (var i = 0; i < 100; i++)
            {
                ai.Tick();
            }
        }

        [Test]
        public void StateMachine_Test()
        {
            var ai = StateMachineMiner.BuildAI();
            for (var i = 0; i < 100; i++)
            {
                ai.Tick();
            }
        }
    }
}
