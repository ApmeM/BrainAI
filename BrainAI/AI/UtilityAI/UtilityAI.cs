namespace BrainAI.AI.UtilityAI
{
    using BrainAI.AI;

    public class UtilityAI<T> : IAITurn
    {
        /// <summary>
        /// The context should contain all the data needed to run the tree
        /// </summary>
        private readonly T context;

        private readonly IAction<T> rootAction;
        private IAction<T> lastAction;

        public UtilityAI( T context, Reasoner<T> rootSelector )
        {
            this.rootAction = new ReasonerAction<T>(rootSelector, (a)=>lastAction = a, ()=>lastAction);
            this.context = context;
        }

        public void Tick()
        {
            this.rootAction.Execute(this.context);
        }
    }
}

