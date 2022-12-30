namespace BrainAI.Tests.AI
{
    using System;

    using BrainAI.AI.BehaviorTrees;

    /// <summary>
    /// implements our friend miner bob with behavior trees. The same tree is built using self abort and lower priority types to illustrate
    /// different ways of using behavior trees.
    /// </summary>
    public class BehaviorTreeMiner
    {
        private static Random r = new Random();

        public class BehaviorTreeMinerState
        {

            public MinerState MinerState = new MinerState();

            public int DistanceToNextLocation = 10;

            public TaskStatus GoToLocation(MinerState.Location location)
            {
                Console.WriteLine($"Heading to {location}. Its {this.DistanceToNextLocation} miles away");

                if (location != this.MinerState.CurrentLocation)
                {
                    this.DistanceToNextLocation--;
                    if (this.DistanceToNextLocation == 0)
                    {
                        this.MinerState.Fatigue++;
                        this.MinerState.CurrentLocation = location;
                        this.DistanceToNextLocation = r.Next(6) + 2;

                        return TaskStatus.Success;
                    }

                    return TaskStatus.Running;
                }
                return TaskStatus.Success;
            }

            public TaskStatus Sleep()
            {
                Console.WriteLine($"Getting some sleep. Current fatigue {this.MinerState.Fatigue}");

                if (this.MinerState.Fatigue == 0)
                    return TaskStatus.Success;

                this.MinerState.Fatigue--;
                return TaskStatus.Running;
            }

            public TaskStatus Drink()
            {
                Console.WriteLine($"Getting my drink on. Thirst level {this.MinerState.Thirst}");

                if (this.MinerState.Thirst == 0)
                    return TaskStatus.Success;

                this.MinerState.Thirst--;
                return TaskStatus.Running;
            }

            public TaskStatus DepositGold()
            {
                this.MinerState.GoldInBank += this.MinerState.Gold;
                this.MinerState.Gold = 0;

                Console.WriteLine($"Depositing gold at the bank. Current wealth {this.MinerState.GoldInBank}");

                return TaskStatus.Success;
            }

            public TaskStatus DigForGold()
            {
                Console.WriteLine($"Digging for gold. Nuggets found {this.MinerState.Gold}");
                this.MinerState.Gold++;
                this.MinerState.Fatigue++;
                this.MinerState.Thirst++;

                if (this.MinerState.Gold >= MinerState.MAX_GOLD)
                    return TaskStatus.Failure;

                return TaskStatus.Running;
            }
        }


        public static BehaviorTree<BehaviorTreeMinerState> BuildSelfAbortTree()
        {
            var builder = BehaviorTreeBuilder<BehaviorTreeMinerState>.Begin( new BehaviorTreeMinerState() );

            builder.Selector( AbortTypes.Self );

            // sleep is most important
            builder.ConditionalDecorator( m => m.MinerState.Fatigue >= MinerState.MAX_FATIGUE, false );
            builder.Sequence()
                .LogAction( "--- Tired! Gotta go home" )
                .Action( m => m.GoToLocation( MinerState.Location.Home ) )
                .LogAction( "--- Prep me my bed!" )
                .Action( m => m.Sleep() )
                .EndComposite();

            // thirst is next most important
            builder.ConditionalDecorator( m => m.MinerState.Thirst >= MinerState.MAX_THIRST, false );
            builder.Sequence()
                .LogAction( "--- Thirsty! Time for a drink" )
                .Action( m => m.GoToLocation( MinerState.Location.Saloon ) )
                .LogAction( "--- Get me a drink!" )
                .Action( m => m.Drink() )
                .EndComposite();

            // dropping off gold is next
            builder.ConditionalDecorator( m => m.MinerState.Gold >= MinerState.MAX_GOLD, false );
            builder.Sequence()
                .LogAction( "--- Bags are full! Gotta drop this off at the bank." )
                .Action( m => m.GoToLocation( MinerState.Location.Bank ) )
                .LogAction( "--- Take me gold!" )
                .Action( m => m.DepositGold() )
                .EndComposite();

            // fetching gold is last
            builder.Sequence()
                .Action( m => m.GoToLocation( MinerState.Location.Mine ) )
                .LogAction( "--- Time to get me some gold!" )
                .Action( m => m.DigForGold() )
                .EndComposite();

            builder.EndComposite();

            return builder.Build();
        }


        // the same tree is here, once with LowerPriority aborts and once with Self aborts and ConditionalDecorators
        public static BehaviorTree<BehaviorTreeMinerState> BuildLowerPriorityAbortTree()
        {
            var builder = BehaviorTreeBuilder<BehaviorTreeMinerState>.Begin( new BehaviorTreeMinerState() );

            builder.Selector();

            // sleep is most important
            builder.Sequence( AbortTypes.LowerPriority )
                .Conditional( m => m.MinerState.Fatigue >= MinerState.MAX_FATIGUE )
                .LogAction( "--- Tired! Gotta go home" )
                .Action( m => m.GoToLocation( MinerState.Location.Home ) )
                .LogAction( "--- Prep me my bed!" )
                .Action( m => m.Sleep() )
                .EndComposite();

            // thirst is next most important
            builder.Sequence( AbortTypes.LowerPriority )
                .Conditional( m => m.MinerState.Thirst >= MinerState.MAX_THIRST )
                .LogAction( "--- Thirsty! Time for a drink" )
                .Action( m => m.GoToLocation( MinerState.Location.Saloon ) )
                .LogAction( "--- Get me a drink!" )
                .Action( m => m.Drink() )
                .EndComposite();

            // dropping off gold is next
            builder.Sequence( AbortTypes.LowerPriority )
                .Conditional( m => m.MinerState.Gold >= MinerState.MAX_GOLD )
                .LogAction( "--- Bags are full! Gotta drop this off at the bank." )
                .Action( m => m.GoToLocation( MinerState.Location.Bank ) )
                .LogAction( "--- Take me gold!" )
                .Action( m => m.DepositGold() )
                .EndComposite();

            // fetching gold is last
            builder.Sequence()
                .Action( m => m.GoToLocation( MinerState.Location.Mine ) )
                .LogAction( "--- Time to get me some gold!" )
                .Action( m => m.DigForGold() )
                .EndComposite();

            builder.EndComposite();

            return builder.Build();
        }
    }
    public static class BehaviorTreeBuilderExtension
    {
        public static BehaviorTreeBuilder<T> LogAction<T>(this BehaviorTreeBuilder<T> builder, string text)
        {
            return builder.AddChildBehavior(new BehaviorTreeLogAction<T>(text));
        }
    }

    /// <summary>
    /// simple task which will output the specified text and return success. It can be used for debugging.
    /// </summary>
    public class BehaviorTreeLogAction<T> : Behavior<T>
    {
        /// <summary>
        /// text to log
        /// </summary>
        public string Text;

        /// <summary>
        /// is this text an error
        /// </summary>
        public bool IsError;


        public BehaviorTreeLogAction(string text)
        {
            this.Text = text;
        }


        public override TaskStatus Update(T context)
        {
            if (this.IsError)
                Console.WriteLine($"ERROR: {this.Text}");
            else
                Console.WriteLine($"INFO: {this.Text}");

            return TaskStatus.Success;
        }
    }
}

