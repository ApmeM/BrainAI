namespace BrainAI.AI.UtilityAI.Actions
{
    using BrainAI.AI.UtilityAI.Reasoners;

    /// <summary>
    /// Action that calls through to another Reasoner
    /// </summary>
    public class ReasonerAction<T> : IAction<T>
    {
        private readonly Reasoner<T> reasoner;

        public ReasonerAction( Reasoner<T> reasoner )
        {
            this.reasoner = reasoner;
        }

        void IAction<T>.Execute( T context )
        {
            var action = this.reasoner.Select( context );
            action?.Execute( context );
        }
    }
}

