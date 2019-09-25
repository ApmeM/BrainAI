namespace BrainAI.AI.GOAP
{
    using System.Collections.Generic;

    /// <summary>
    /// Agent provides a simple and concise way to use the planner. It is not necessary to use at all since it is just a convenince wrapper
    /// around the ActionPlanner making it easier to get plans and store the results.
    /// </summary>
    public abstract class Agent
    {
        public Stack<GOAPAction> Actions;
        protected ActionPlanner Planner;

        protected Agent()
        {
            this.Planner = new ActionPlanner();
        }

        public bool Plan( bool debugPlan = false )
        {
            List<GOAPNode> nodes = null;
            if( debugPlan )
                nodes = new List<GOAPNode>();
            
            this.Actions = this.Planner.Plan( this.GetWorldState(), this.GetGoalState(), nodes );

            if (nodes == null || nodes.Count <= 0)
            {
                return this.HasActionPlan();
            }

            //---- ActionPlanner plan ----
            //plan cost = {nodes[nodes.Count - 1].CostSoFar}
            //start    {this.GetWorldState().Describe(this.Planner)}
            //for ( var i = 0; i < nodes.Count; i++ )
            //{
                //{i}: {nodes[i].Action.GetType().Name}    {nodes[i].WorldState.Describe(this.Planner)}"
            //}

            return this.HasActionPlan();
        }


        public bool HasActionPlan()
        {
            return this.Actions != null && this.Actions.Count > 0;
        }


        /// <summary>
        /// current WorldState
        /// </summary>
        /// <returns>The world state.</returns>
        public abstract WorldState GetWorldState();


        /// <summary>
        /// the goal state that the agent wants to achieve
        /// </summary>
        /// <returns>The goal state.</returns>
        public abstract WorldState GetGoalState();
    }
}

