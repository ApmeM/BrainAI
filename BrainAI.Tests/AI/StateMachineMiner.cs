namespace BrainAI.Tests.AI
{
    using System;

    using BrainAI.AI;
    using BrainAI.AI.FSM;

    public class StateMachineMiner
    {
        public static IAITurn BuildAI()
        {
            return new MinerStateMachine();
        }

        public class MinerStateMachine : StateMachine<MinerState>
        {
            public MinerStateMachine() : base(new MinerState(), new MineState())
            {
                this.AddState(new SleepState());
                this.AddState(new DrinkState());
            }
        }

        private class MineState: State<MinerState>
        {
            public override void Update()
            {
                if (this.Context.Fatigue >= MinerState.MAX_FATIGUE)
                {
                    this.Machine.ChangeState<SleepState>();
                    return;
                }

                if (this.Context.Thirst >= MinerState.MAX_THIRST)
                {
                    this.Machine.ChangeState<DrinkState>();
                    return;
                }
                Console.WriteLine($"Digging for gold. Nuggets found {this.Context.Gold}");
                this.Context.Gold++;
                this.Context.Fatigue++;
                this.Context.Thirst++;
            }
        }

        private class SleepState: State<MinerState>
        {
            public override void Update()
            {
                if (this.Context.Fatigue == 0)
                {
                    this.Machine.ChangeState<MineState>();
                }

                Console.WriteLine($"Getting some sleep. Current fatigue {this.Context.Fatigue}");
                this.Context.Fatigue--;
            }
        }

        private class DrinkState: State<MinerState>
        {
            public override void Update()
            {
                if (this.Context.Thirst == 0)
                {
                    this.Machine.ChangeState<MineState>();
                }

                Console.WriteLine($"Getting my drink on. Thirst level {this.Context.Thirst}");
                this.Context.Thirst--;
            }
        }
    }
}