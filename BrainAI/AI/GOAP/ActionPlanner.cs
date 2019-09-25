namespace BrainAI.AI.GOAP
{
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// GOAP based on https://github.com/stolk/GPGOAP
    /// </summary>
    public class ActionPlanner
    {
        public const int MaxConditions = 64;

        /// <summary>
        /// Names associated with all world state atoms
        /// </summary>
        public string[] ConditionNames = new string[MaxConditions];

        private readonly List<GOAPAction> actions = new List<GOAPAction>();

        private readonly List<GOAPAction> viableActions = new List<GOAPAction>();

        /// <summary>
        /// Preconditions for all actions
        /// </summary>
        private readonly WorldState[] preConditions = new WorldState[MaxConditions];

        /// <summary>
        /// Postconditions for all actions (action effects).
        /// </summary>
        private readonly WorldState[] postConditions = new WorldState[MaxConditions];

        /// <summary>
        /// Number of world state atoms.
        /// </summary>
        private int numConditionNames;


        public ActionPlanner()
        {
            this.numConditionNames = 0;
            for( var i = 0; i < MaxConditions; ++i )
            {
                this.ConditionNames[i] = null;
                this.preConditions[i] = WorldState.Create( this );
                this.postConditions[i] = WorldState.Create( this );
            }
        }


        /// <summary>
        /// convenince method for fetching a WorldState object
        /// </summary>
        /// <returns>The world state.</returns>
        public WorldState CreateWorldState()
        {
            return WorldState.Create( this );
        }


        public void AddAction( GOAPAction goapAction )
        {
            var actionId = this.FindActionIndex( goapAction );
            if( actionId == -1 )
                throw new KeyNotFoundException( "could not find or create Action" );

            foreach( var preCondition in goapAction.PreConditions )
            {
                var conditionId = this.FindConditionNameIndex( preCondition.Item1 );
                if( conditionId == -1 )
                    throw new KeyNotFoundException( "could not find or create conditionName" );

                this.preConditions[actionId].Set( conditionId, preCondition.Item2 );
            }

            foreach( var postCondition in goapAction.PostConditions )
            {
                var conditionId = this.FindConditionNameIndex( postCondition.Item1 );
                if( conditionId == -1 )
                    throw new KeyNotFoundException( "could not find conditionName" );

                this.postConditions[actionId].Set( conditionId, postCondition.Item2 );
            }
        }


        public Stack<GOAPAction> Plan( WorldState startState, WorldState goalState, List<GOAPNode> selectedNodes = null )
        {
            this.viableActions.Clear();
            for( var i = 0; i < this.actions.Count; i++ )
            {
                if( this.actions[i].Validate() )
                    this.viableActions.Add( this.actions[i] );
            }

            return GOAPWorld.Plan( this, startState, goalState, selectedNodes );
        }


        /// <summary>
        /// Describe the action planner by listing all actions with pre and post conditions. For debugging purpose.
        /// </summary>
        public string Describe()
        {
            var sb = new StringBuilder();
            for( var a = 0; a < this.actions.Count; ++a )
            {
                sb.AppendLine( this.actions[a].GetType().Name );

                var pre = this.preConditions[a];
                var pst = this.postConditions[a];
                for( var i = 0; i < MaxConditions; ++i )
                {
                    if( ( pre.DontCare & ( 1L << i ) ) == 0 )
                    {
                        bool v = ( pre.Values & ( 1L << i ) ) != 0;
                        sb.AppendFormat( "  {0}=={1}\n", this.ConditionNames[i], v ? 1 : 0 );
                    }
                }

                for( var i = 0; i < MaxConditions; ++i )
                {
                    if( ( pst.DontCare & ( 1L << i ) ) == 0 )
                    {
                        bool v = ( pst.Values & ( 1L << i ) ) != 0;
                        sb.AppendFormat( "  {0}:={1}\n", this.ConditionNames[i], v ? 1 : 0 );
                    }
                }
            }

            return sb.ToString();
        }


        internal int FindConditionNameIndex( string conditionName )
        {
            int idx;
            for( idx = 0; idx < this.numConditionNames; ++idx )
            {
                if( string.Equals( this.ConditionNames[idx], conditionName ) )
                    return idx;
            }

            if( idx < MaxConditions - 1 )
            {
                this.ConditionNames[idx] = conditionName;
                this.numConditionNames++;
                return idx;
            }

            return -1;
        }


        internal int FindActionIndex( GOAPAction goapAction )
        {
            var idx = this.actions.IndexOf( goapAction );
            if( idx > -1 )
                return idx;

            this.actions.Add( goapAction );

            return this.actions.Count - 1;
        }


        internal List<GOAPNode> GetPossibleTransitions( WorldState fr )
        {
            var result = new List<GOAPNode>();
            for( var i = 0; i < this.viableActions.Count; ++i )
            {
                // see if precondition is met
                var pre = this.preConditions[i];
                var care = ( pre.DontCare ^ -1L );
                bool met = ( ( pre.Values & care ) == ( fr.Values & care ) );
                if( met )
                {
                    var node = new GOAPNode
                    {
                        Action = this.viableActions[i],
                        CostSoFar = this.viableActions[i].Cost,
                        WorldState = this.ApplyPostConditions(this, i, fr)
                    };
                    result.Add( node );
                }
            }
            return result;
        }


        internal WorldState ApplyPostConditions( ActionPlanner ap, int actionnr, WorldState fr )
        {
            var pst = ap.postConditions[actionnr];
            long unaffected = pst.DontCare;
            long affected = ( unaffected ^ -1L );

            fr.Values = ( fr.Values & unaffected ) | ( pst.Values & affected );
            fr.DontCare &= pst.DontCare;
            return fr;
        }
    }
}

