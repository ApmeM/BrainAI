namespace BrainAI.Sample.AI
{
    using System;
    using System.Collections.Generic;

    using BrainAI.AI.FSM;
    using BrainAI.AI.GOAP;

    /// <summary>
    /// Goal Oriented Action Planning miner bob. Sets up Actions and uses the ActionPlanner to pick the appropriate action based on the world
    /// and goal states. This example also uses a SimpleStateMachine to deal with exeucuting the plan that the ActionPlanner comes up with.
    /// </summary>
    public class GOAPMiner
    {
        private const string IsFatigued = "fatigued";

        private const string IsThirsty = "thirsty";

        private const string HasEnoughGold = "hasenoughgold";

        private static Random r = new Random();

        public class GOAPMinerState
        {
            public MinerState MinerState = new MinerState();

            public MinerState.Location DestinationLocation;

            public int DistanceToNextLocation = 10;

            public Stack<GOAPAction> ActionPlan;

            public ActionPlanner Planner;

            public WorldState GetWorldState()
            {
                var worldState = this.Planner.CreateWorldState();
                worldState.Set(IsFatigued, this.MinerState.Fatigue >= MinerState.MAX_FATIGUE);
                worldState.Set(IsThirsty, this.MinerState.Thirst >= MinerState.MAX_THIRST);
                worldState.Set(HasEnoughGold, this.MinerState.Gold >= MinerState.MAX_GOLD);

                return worldState;
            }

            public WorldState GetGoalState()
            {
                var goalState = this.Planner.CreateWorldState();

                if (this.MinerState.Fatigue >= MinerState.MAX_FATIGUE)
                    goalState.Set(IsFatigued, false);
                else if (this.MinerState.Thirst >= MinerState.MAX_THIRST)
                    goalState.Set(IsThirsty, false);
                else if (this.MinerState.Gold >= MinerState.MAX_GOLD)
                    goalState.Set(HasEnoughGold, false);
                else
                    goalState.Set(HasEnoughGold, true);

                return goalState;
            }
        }

        public static StateMachine<GOAPMinerState> BuildAI()
        {
            var goapMinerState = new GOAPMinerState();
            
            goapMinerState.Planner = new ActionPlanner();

            // setup our Actions and add them to the planner
            var sleep = new GOAPAction("sleep");
            sleep.SetPrecondition(IsFatigued, true);
            sleep.SetPostcondition(IsFatigued, false);
            goapMinerState.Planner.AddAction(sleep);

            var drink = new GOAPAction("drink");
            drink.SetPrecondition(IsThirsty, true);
            drink.SetPostcondition(IsThirsty, false);
            goapMinerState.Planner.AddAction(drink);

            var mine = new GOAPAction("mine");
            mine.SetPrecondition(HasEnoughGold, false);
            mine.SetPostcondition(HasEnoughGold, true);
            goapMinerState.Planner.AddAction(mine);

            var depositGold = new GOAPAction("depositGold");
            depositGold.SetPrecondition(HasEnoughGold, true);
            depositGold.SetPostcondition(HasEnoughGold, false);
            goapMinerState.Planner.AddAction(depositGold);

            var stateMachine = new StateMachine<GOAPMinerState>(goapMinerState, new Idle());
            stateMachine.AddState(new GoTo());
            stateMachine.AddState(new PerformAction());

            return stateMachine;
        }

        #region states

        public class Idle : State<GOAPMinerState>
        {
            public override void Begin()
            {
                base.Begin();
                // get a plan to run that will get us from our current state to our goal state
                this.Context.ActionPlan = this.Context.Planner.Plan(this.Context.GetWorldState(), this.Context.GetGoalState());

                if (this.Context.ActionPlan != null && this.Context.ActionPlan.Count > 0)
                {
                    this.Machine.ChangeState<GoTo>();
                    Console.WriteLine($"Got an action plan with {this.Context.ActionPlan.Count} actions");
                }
                else
                {
                    Console.WriteLine($"No action plan satisfied our goals");
                }
            }

            public override void Update()
            {
            }
        }


        public class GoTo : State<GOAPMinerState>
        {
            public override void Begin()
            {

                // figure out where we are going
                var action = this.Context.ActionPlan.Peek().Name;
                switch (action)
                {
                    case "sleep":
                        this.Context.DestinationLocation = MinerState.Location.Home;
                        break;
                    case "drink":
                        this.Context.DestinationLocation = MinerState.Location.Saloon;
                        break;
                    case "mine":
                        this.Context.DestinationLocation = MinerState.Location.Mine;
                        break;
                    case "depositGold":
                        this.Context.DestinationLocation = MinerState.Location.Bank;
                        break;
                }

                Console.WriteLine($"Start heading to {this.Context.DestinationLocation}");
                if (this.Context.MinerState.CurrentLocation == this.Context.DestinationLocation)
                {
                    this.Machine.ChangeState<PerformAction>();
                }
                else
                {
                    this.Context.DistanceToNextLocation = r.Next(6) + 2;
                    this.Context.MinerState.CurrentLocation = MinerState.Location.InTransit;
                }
            }


            public override void Update()
            {
                Console.WriteLine($"Heading to {this.Context.DestinationLocation}. Its {this.Context.DistanceToNextLocation} miles away");
                this.Context.DistanceToNextLocation--;

                if (this.Context.DistanceToNextLocation == 0)
                {
                    this.Context.MinerState.Fatigue++;
                    this.Context.MinerState.CurrentLocation = this.Context.DestinationLocation;
                    this.Machine.ChangeState<PerformAction>();
                }
            }

            public override void End()
            {
                Console.WriteLine($"Made it to the {this.Context.MinerState.CurrentLocation}");
            }
        }

        public class PerformAction : State<GOAPMinerState>
        {
            public override void Update()
            {
                var action = this.Context.ActionPlan.Peek().Name;

                switch (action)
                {
                    case "sleep":
                        Console.WriteLine($"Getting some sleep. Current fatigue {this.Context.MinerState.Fatigue}");
                        this.Context.MinerState.Fatigue--;

                        if (this.Context.MinerState.Fatigue == 0)
                            this.Machine.ChangeState<Idle>();
                        break;
                    case "drink":
                        Console.WriteLine($"Getting my drink on. Thirst level {this.Context.MinerState.Thirst}");
                        this.Context.MinerState.Thirst--;

                        if (this.Context.MinerState.Thirst == 0)
                            this.Machine.ChangeState<Idle>();
                        break;
                    case "mine":
                        Console.WriteLine($"Digging for gold. Nuggets found {this.Context.MinerState.Gold}");
                        this.Context.MinerState.Gold++;
                        this.Context.MinerState.Fatigue++;
                        this.Context.MinerState.Thirst++;

                        if (this.Context.MinerState.Gold >= MinerState.MAX_GOLD)
                            this.Machine.ChangeState<Idle>();
                        break;
                    case "depositGold":
                        this.Context.MinerState.GoldInBank += this.Context.MinerState.Gold;
                        this.Context.MinerState.Gold = 0;

                        Console.WriteLine($"Depositing gold at the bank. Current wealth {this.Context.MinerState.GoldInBank}");
                        this.Machine.ChangeState<Idle>();
                        break;
                }
            }
        }

        #endregion

    }
}

