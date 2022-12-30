namespace BrainAI.AI.UtilityAI
{
    using BrainAI.AI;

    public class UtilityAI<T> : IAITurn
    {
        /// <summary>
        /// The context should contain all the data needed to run the tree
        /// </summary>
        private readonly T context;

        private readonly Reasoner<T> rootReasoner;

        public UtilityAI( T context, Reasoner<T> rootSelector )
        {
            this.rootReasoner = rootSelector;
            this.context = context;
        }

        public void Tick()
        {
            var action = this.rootReasoner.SelectBestAction( this.context );
            action?.Execute( this.context );
        }
    }
}

