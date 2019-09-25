namespace BrainAI.AI.UtilityAI.Reasoners
{
    using System.Collections.Generic;

    using BrainAI.AI.UtilityAI.Actions;
    using BrainAI.AI.UtilityAI.Considerations;

    /// <summary>
    /// the root of UtilityAI.
    /// </summary>
    public abstract class Reasoner<T>
    {
        public IConsideration<T> DefaultConsideration = new FixedScoreConsideration<T>();

        protected List<IConsideration<T>> Considerations = new List<IConsideration<T>>();


        public IAction<T> Select( T context )
        {
            var consideration = this.SelectBestConsideration( context );
            return consideration?.Action;
        }


        protected abstract IConsideration<T> SelectBestConsideration( T context );


        public Reasoner<T> AddConsideration( IConsideration<T> consideration )
        {
            this.Considerations.Add( consideration );
            return this;
        }


        public Reasoner<T> SetDefaultConsideration( IConsideration<T> defaultConsideration )
        {
            this.DefaultConsideration = defaultConsideration;
            return this;
        }

    }
}

