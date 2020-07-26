namespace BrainAI.Tests.AI
{
    using System;
    using System.Collections.Generic;

    using BrainAI.AI.UtilityAI;
    using BrainAI.AI.UtilityAI.Actions;
    using BrainAI.AI.UtilityAI.Considerations;
    using BrainAI.AI.UtilityAI.Considerations.Appraisals;
    using BrainAI.AI.UtilityAI.Reasoners;

    /// <summary>
    /// Utility AI example of miner bob. Utility AI is the most complex of all the AI types to setup. The complexity comes with a lot of power
    /// though.
    /// </summary>
    public class UtilityMiner
    {
        private static Random r = new Random();

        public class UtilityMinerState
        {
            public MinerState MinerState = new MinerState();
            public MinerState.Location DestinationLocation;
            public int DistanceToNextLocation = 10;

            public void Sleep()
            {
                Console.WriteLine($"Getting some sleep. Current fatigue {this.MinerState.Fatigue}");
                this.MinerState.Fatigue--;
            }

            public void Drink()
            {
                Console.WriteLine($"Getting my drink on. Thirst level {this.MinerState.Thirst}");
                this.MinerState.Thirst--;
            }

            public void DepositGold()
            {
                this.MinerState.GoldInBank += this.MinerState.Gold;
                this.MinerState.Gold = 0;

                Console.WriteLine($"Depositing gold at the bank. Current wealth {this.MinerState.GoldInBank}");
            }

            public void DigForGold()
            {
                Console.WriteLine($"Digging for gold. Nuggets found {this.MinerState.Gold}");
                this.MinerState.Gold++;
                this.MinerState.Fatigue++;
                this.MinerState.Thirst++;
            }

            public void GoToLocation(MinerState.Location location)
            {
                if (location == this.MinerState.CurrentLocation)
                    return;

                if (this.MinerState.CurrentLocation == MinerState.Location.InTransit && location == this.DestinationLocation)
                {
                    Console.WriteLine($"Heading to {location}. Its {this.DistanceToNextLocation} miles away");
                    this.DistanceToNextLocation--;
                    if (this.DistanceToNextLocation == 0)
                    {
                        this.MinerState.Fatigue++;
                        this.MinerState.CurrentLocation = this.DestinationLocation;
                        this.DistanceToNextLocation = r.Next(6) + 2;
                    }
                }
                else
                {
                    this.MinerState.CurrentLocation = MinerState.Location.InTransit;
                    this.DestinationLocation = location;
                    this.DistanceToNextLocation = r.Next(6) + 2;
                }
            }
        }

        public static UtilityAI<UtilityMinerState> BuildAI()
        {
            var reasoner = new FirstScoreReasoner<UtilityMinerState>();

            // sleep is most important
            // AllOrNothingQualifier with required threshold of 1 so all scorers must score
            //  - we have to be home to sleep
            //  - we have to have some fatigue
            var fatigueConsideration = new AllOrNothingConsideration<UtilityMinerState>(1);
            fatigueConsideration.Appraisals.Add(new ActionAppraisal<UtilityMinerState>(c => c.MinerState.CurrentLocation == MinerState.Location.Home ? 1 : 0));
            fatigueConsideration.Appraisals.Add(new ActionAppraisal<UtilityMinerState>(c => c.MinerState.Fatigue > 0 ? 1 : 0));
            fatigueConsideration.Action = new ActionExecutor<UtilityMinerState>( c => c.Sleep() );
            reasoner.AddConsideration( fatigueConsideration );

            // thirst is next
            // AllOrNothingQualifier with required threshold of 1 so all scorers must score
            //  - we have to be at the saloon to drink
            //  - we have to be thirsty
            var thirstConsideration = new AllOrNothingConsideration<UtilityMinerState>(1);
            thirstConsideration.Appraisals.Add(new ActionAppraisal<UtilityMinerState>( c => c.MinerState.CurrentLocation == MinerState.Location.Saloon ? 1 : 0));
            thirstConsideration.Appraisals.Add(new ActionAppraisal<UtilityMinerState>(c => c.MinerState.Thirst > 0 ? 1 : 0));
            thirstConsideration.Action = new ActionExecutor<UtilityMinerState>( c => c.Drink() );
            reasoner.AddConsideration( thirstConsideration );

            // depositing gold is next
            // AllOrNothingQualifier with required threshold of 1 so all scorers must score
            //  - we have to be at the bank to deposit gold
            //  - we have to have gold to deposit
            var goldConsideration = new AllOrNothingConsideration<UtilityMinerState>(1);
            goldConsideration.Appraisals.Add( new ActionAppraisal<UtilityMinerState>( c => c.MinerState.CurrentLocation == MinerState.Location.Bank ? 1 : 0 ));
            goldConsideration.Appraisals.Add( new ActionAppraisal<UtilityMinerState>( c => c.MinerState.Gold > 0 ? 1 : 0 ) );
            goldConsideration.Action = new ActionExecutor<UtilityMinerState>( c => c.DepositGold() );
            reasoner.AddConsideration( goldConsideration );

            // decide where to go. this will override mining and send us elsewhere if a scorer scores
            // AllOrNothingQualifier with required threshold of 0 so we get a sum of all scorers
            //  - if we are max fatigued score
            //  - if we are max thirsty score
            //  - if we are at max gold score
            //  - if we are not at the mine score
            // Action has a scorer to score all the locations. It then moves to the location that scored highest.
            var moveConsideration = new AllOrNothingConsideration<UtilityMinerState>(0);
            moveConsideration.Appraisals.Add(new ActionAppraisal<UtilityMinerState>(c => c.MinerState.Fatigue >= MinerState.MAX_FATIGUE ? 1 : 0));
            moveConsideration.Appraisals.Add(new ActionAppraisal<UtilityMinerState>(c => c.MinerState.Thirst >= MinerState.MAX_THIRST ? 1 : 0));
            moveConsideration.Appraisals.Add(new ActionAppraisal<UtilityMinerState>(c => c.MinerState.Gold >= MinerState.MAX_GOLD ? 1 : 0));
            moveConsideration.Appraisals.Add(new ActionAppraisal<UtilityMinerState>(c => c.MinerState.CurrentLocation != MinerState.Location.Mine ? 1 : 0));
            var moveAction = new MoveToBestLocation();
            moveAction.Appraisals.Add( new ChooseBestLocation() );
            moveConsideration.Action = moveAction;
            reasoner.AddConsideration( moveConsideration );

            // mining is last
            // AllOrNothingQualifier with required threshold of 1 so all scorers must score
            //  - we have to be at the mine to dig for gold
            //  - we have to not be at our max gold
            var mineConsideration = new AllOrNothingConsideration<UtilityMinerState>(1);
            mineConsideration.Appraisals.Add(new ActionAppraisal<UtilityMinerState>(c => c.MinerState.CurrentLocation == MinerState.Location.Mine ? 1 : 0));
            mineConsideration.Appraisals.Add(new ActionAppraisal<UtilityMinerState>(c => c.MinerState.Gold >= MinerState.MAX_GOLD ? 0 : 1));
            mineConsideration.Action = new ActionExecutor<UtilityMinerState>( c => c.DigForGold() );
            reasoner.AddConsideration( mineConsideration );

            // default, fall-through action is to head to the mine
            reasoner.DefaultConsideration.Action = new ActionExecutor<UtilityMinerState>( c => c.GoToLocation( MinerState.Location.Mine ) );

            return new UtilityAI<UtilityMinerState>( new UtilityMinerState(), reasoner );
        }
    }
    
    public class ChooseBestLocation : IActionOptionAppraisal<UtilityMiner.UtilityMinerState,MinerState.Location>
    {
        /// <summary>
        /// Action Appraisal that will score locations providing the highest score to the best location to visit
        /// </summary>
        /// <returns>The score.</returns>
        /// <param name="context">Context.</param>
        /// <param name="option">Option.</param>
        public float GetScore(UtilityMiner.UtilityMinerState context, MinerState.Location option )
        {
            if( option == MinerState.Location.Home )
                return context.MinerState.Fatigue >= MinerState.MAX_FATIGUE ? 20 : 0;

            if( option == MinerState.Location.Saloon )
                return context.MinerState.Thirst >= MinerState.MAX_THIRST ? 15 : 0;

            if( option == MinerState.Location.Bank )
            {
                if( context.MinerState.Gold >= MinerState.MAX_GOLD )
                    return 10;

                // if we are scoring the bank and we are not at the mine we'll use a curve. the main gist of this is that if we are not at the mine
                // and we are carrying a decent amount of gold drop it off at the bank before heading to the mine again.
                if( context.MinerState.CurrentLocation != MinerState.Location.Mine )
                {
                    // normalize our current gold value to 0-1
                    var gold = context.MinerState.Gold / (double)MinerState.MAX_GOLD;
                    var score = Math.Pow( gold, 2 );
                    return (float)score * 10;
                }

                return 0;
            }

            return 5;
        }

    }
    /// <summary>
    /// ActionWithOptions lets an Action setup an Appraisal that will score a list of options. In our miner bob example, the options
    /// are the locations and the Appraisal will score the best location to go to.
    /// </summary>
    public class MoveToBestLocation : ActionWithOptions<UtilityMiner.UtilityMinerState, MinerState.Location>
    {
        private readonly List<MinerState.Location> locations = new List<MinerState.Location>()
        {
            MinerState.Location.Bank,
            MinerState.Location.Home,
            MinerState.Location.Mine,
            MinerState.Location.Saloon
        };


        public override void Execute(UtilityMiner.UtilityMinerState context )
        {
            var location = this.GetBestOption( context, this.locations );

            context.GoToLocation( location );
        }

    }
}

