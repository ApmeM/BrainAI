using System;

namespace BrainAI.Sample
{
    using System.Threading;

    using BrainAI.Sample.AI;

    internal class Program
    {
        static void Main(string[] args)
        {
            int? aiType;
            while ((aiType = SelectAIType()) == null){}

            IAITurn ai;
            switch (aiType.Value)
            {
                case 1:
                    ai = UtilityMiner.BuildAI();
                    break;
                case 2:
                    ai = BehaviorTreeMiner.BuildSelfAbortTree();
                    break;
                case 3:
                    ai = GOAPMiner.BuildAI();
                    break;
                default:
                    throw new NotSupportedException($"AI type {aiType.Value} not supported.");
            }

            while (true)
            {
                ai.Tick();
                Thread.Sleep(1000);
            }
        }

        private static int? SelectAIType()
        {
            Console.WriteLine("Choose AI Implementation:");
            Console.WriteLine("1 - Utility");
            Console.WriteLine("2 - Behavior tree");
            Console.WriteLine("3 - GOAP");
            if (int.TryParse(Console.ReadLine(), out int result))
            {
                return result;
            }

            return null;
        }

    }
}
