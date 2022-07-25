namespace BrainAI.AI.UtilityAI.Reasoners
{
    using System.Collections.Generic;
    using BrainAI.AI.UtilityAI.Appraisals;
    using BrainAI.AI.UtilityAI.Actions;

    /// <summary>
    /// the root of UtilityAI.
    /// </summary>
    public abstract partial class Reasoner<T>
    {
        protected class Consideration
        {
            public IAppraisal<T> Appraisal { get; set; }
            public IAction<T> Action { get; set; }
        }

        protected readonly List<Consideration> Considerations = new List<Consideration>();

        public abstract IAction<T> SelectBestAction(T context);

        public Reasoner<T> Add(IAppraisal<T> appraisal, params IAction<T>[] actions)
        {
            this.Considerations.Add(new Consideration
            {
                Appraisal = appraisal,
                Action = new CompositeAction<T>(actions)
            });

            return this;
        }
    }
}

