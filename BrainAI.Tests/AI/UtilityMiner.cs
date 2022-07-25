namespace BrainAI.Tests.AI
{
    using System;
    using System.Collections.Generic;

    using BrainAI.AI.UtilityAI;
    using BrainAI.AI.UtilityAI.Actions;
    using BrainAI.AI.UtilityAI.Appraisals;
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
            var reasoner = new FirstScoreReasoner<UtilityMinerState>(1);

            // sleep is most important
            // AllOrNothingQualifier with required threshold of 1 so all scorers must score
            //  - we have to be home to sleep
            //  - we have to have some fatigue
            var fatigueConsideration = new AllOrNothingAppraisal<UtilityMinerState>(1);
            fatigueConsideration.Appraisals.Add(new ActionAppraisal<UtilityMinerState>(c => c.MinerState.CurrentLocation == MinerState.Location.Home ? 1 : 0));
            fatigueConsideration.Appraisals.Add(new ActionAppraisal<UtilityMinerState>(c => c.MinerState.Fatigue > 0 ? 1 : 0));
            reasoner.Add( fatigueConsideration, new ActionAction<UtilityMinerState>( c => c.Sleep() ) );

            // thirst is next
            // AllOrNothingQualifier with required threshold of 1 so all scorers must score
            //  - we have to be at the saloon to drink
            //  - we have to be thirsty
            var thirstConsideration = new AllOrNothingAppraisal<UtilityMinerState>(1);
            thirstConsideration.Appraisals.Add(new ActionAppraisal<UtilityMinerState>( c => c.MinerState.CurrentLocation == MinerState.Location.Saloon ? 1 : 0));
            thirstConsideration.Appraisals.Add(new ActionAppraisal<UtilityMinerState>(c => c.MinerState.Thirst > 0 ? 1 : 0));
            reasoner.Add( thirstConsideration, new ActionAction<UtilityMinerState>( c => c.Drink() ) );

            // depositing gold is next
            // AllOrNothingQualifier with required threshold of 1 so all scorers must score
            //  - we have to be at the bank to deposit gold
            //  - we have to have gold to deposit
            var goldConsideration = new AllOrNothingAppraisal<UtilityMinerState>(1);
            goldConsideration.Appraisals.Add( new ActionAppraisal<UtilityMinerState>( c => c.MinerState.CurrentLocation == MinerState.Location.Bank ? 1 : 0 ));
            goldConsideration.Appraisals.Add( new ActionAppraisal<UtilityMinerState>( c => c.MinerState.Gold > 0 ? 1 : 0 ) );
            reasoner.Add( goldConsideration, new ActionAction<UtilityMinerState>( c => c.DepositGold() ) );

            // decide where to go. this will override mining and send us elsewhere if a scorer scores
            // AllOrNothingQualifier with required threshold of 0 so we get a sum of all scorers
            //  - if we are max fatigued score
            //  - if we are max thirsty score
            //  - if we are at max gold score
            //  - if we are not at the mine score
            // Action has a scorer to score all the locations. It then moves to the location that scored highest.
            var moveConsideration = new AllOrNothingAppraisal<UtilityMinerState>(0);
            moveConsideration.Appraisals.Add(new ActionAppraisal<UtilityMinerState>(c => c.MinerState.Fatigue >= MinerState.MAX_FATIGUE ? 1 : 0));
            moveConsideration.Appraisals.Add(new ActionAppraisal<UtilityMinerState>(c => c.MinerState.Thirst >= MinerState.MAX_THIRST ? 1 : 0));
            moveConsideration.Appraisals.Add(new ActionAppraisal<UtilityMinerState>(c => c.MinerState.Gold >= MinerState.MAX_GOLD ? 1 : 0));
            moveConsideration.Appraisals.Add(new ActionAppraisal<UtilityMinerState>(c => c.MinerState.CurrentLocation != MinerState.Location.Mine ? 1 : 0));
            
            reasoner.Add( moveConsideration, new MoveToBestLocation());

            // mining is last
            // AllOrNothingQualifier with required threshold of 1 so all scorers must score
            //  - we have to be at the mine to dig for gold
            //  - we have to not be at our max gold
            var mineConsideration = new AllOrNothingAppraisal<UtilityMinerState>(1);
            mineConsideration.Appraisals.Add(new ActionAppraisal<UtilityMinerState>(c => c.MinerState.CurrentLocation == MinerState.Location.Mine ? 1 : 0));
            mineConsideration.Appraisals.Add(new ActionAppraisal<UtilityMinerState>(c => c.MinerState.Gold >= MinerState.MAX_GOLD ? 0 : 1));
            reasoner.Add( mineConsideration, new ActionAction<UtilityMinerState>( c => c.DigForGold() ) );

            // default, fall-through action is to head to the mine
            reasoner.Add(
                new FixedScoreAppraisal<UtilityMinerState>(1), 
                new ActionAction<UtilityMinerState>( c => c.GoToLocation( MinerState.Location.Mine ) ));

            return new UtilityAI<UtilityMinerState>( new UtilityMinerState(), reasoner );
        }
    }
    
    public class MoveToBestLocation : IAction<UtilityMiner.UtilityMinerState>
    {
        public void Execute(UtilityMiner.UtilityMinerState context )
        {
            if (context.MinerState.Fatigue >= MinerState.MAX_FATIGUE)
            {
                context.GoToLocation( MinerState.Location.Home );
                return;
            }
            if (context.MinerState.Thirst >= MinerState.MAX_THIRST)
            {
                context.GoToLocation( MinerState.Location.Saloon );
                return;
            }
            if (context.MinerState.Gold >= MinerState.MAX_GOLD)
            {
                context.GoToLocation( MinerState.Location.Bank );
                return;
            }

            if( context.MinerState.CurrentLocation != MinerState.Location.Mine )
            {
                // normalize our current gold value to 0-1
                var gold = context.MinerState.Gold / (double)MinerState.MAX_GOLD;
                var score = Math.Pow( gold, 2 );
                if(score > 0.5){
                    context.GoToLocation( MinerState.Location.Bank );
                    return;
                }
            }

            context.GoToLocation( MinerState.Location.Mine );
        }

    }
}

