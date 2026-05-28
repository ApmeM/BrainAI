namespace BrainAI.Tests.AI
{
    using System;
    using System.Collections;
    using BrainAI.AI.UtilityAI;

    /// <summary>
    /// Coroutine example of miner bob. 
    /// </summary>
    public class CoroutineMiner
    {
        public class CoroutineMinerState
        {
            public MinerState MinerState = new MinerState();
        }

        public static Coroutine BuildAI()
        {
            var context = new CoroutineMinerState();
            context.MinerState.CurrentLocation = MinerState.Location.Mine;
            return new Coroutine(RunMiner(context));
        }

        private static IEnumerator RunMiner(CoroutineMinerState context)
        {
            while (true)
            {
                switch (context.MinerState.CurrentLocation)
                {
                    case MinerState.Location.Mine:
                        yield return Mine(context);
                        break;
                    case MinerState.Location.Home:
                        yield return Sleep(context);
                        break;
                    case MinerState.Location.Saloon:
                        yield return Drink(context);
                        break;
                    default:
                        context.MinerState.CurrentLocation = MinerState.Location.Mine;
                        break;
                }
            }
        }

        private static IEnumerator Mine(CoroutineMinerState context)
        {
            while (true)
            {
                if (context.MinerState.Fatigue >= MinerState.MAX_FATIGUE)
                {
                    context.MinerState.CurrentLocation = MinerState.Location.Home;
                    yield break;
                }

                if (context.MinerState.Thirst >= MinerState.MAX_THIRST)
                {
                    context.MinerState.CurrentLocation = MinerState.Location.Saloon;
                    yield break;
                }

                Console.WriteLine($"Digging for gold. Nuggets found {context.MinerState.Gold}");
                context.MinerState.Gold++;
                context.MinerState.Fatigue++;
                context.MinerState.Thirst++;

                yield return null;
            }
        }

        private static IEnumerator Sleep(CoroutineMinerState context)
        {
            while (true)
            {
                if (context.MinerState.Fatigue == 0)
                {
                    context.MinerState.CurrentLocation = MinerState.Location.Mine;
                    yield break;
                }

                Console.WriteLine($"Getting some sleep. Current fatigue {context.MinerState.Fatigue}");
                context.MinerState.Fatigue--;

                yield return null;
            }
        }

        private static IEnumerator Drink(CoroutineMinerState context)
        {
            while (true)
            {
                if (context.MinerState.Thirst == 0)
                {
                    context.MinerState.CurrentLocation = MinerState.Location.Mine;
                    yield break;
                }

                Console.WriteLine($"Getting my drink on. Thirst level {context.MinerState.Thirst}");
                context.MinerState.Thirst--;

                yield return null;
            }
        }
    }
}

