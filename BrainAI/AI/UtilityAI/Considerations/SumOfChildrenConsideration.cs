namespace BrainAI.AI.UtilityAI.Considerations
{
    using System.Collections.Generic;
    using System.Linq;

    using BrainAI.AI.UtilityAI.Actions;
    using BrainAI.AI.UtilityAI.Considerations.Appraisals;

    /// <summary>
    /// Scores by summing the score of all child Appraisals
    /// </summary>
    public class SumOfChildrenConsideration<T> : IConsideration<T>
    {
        public IAction<T> Action { get; set; }

        public readonly List<IAppraisal<T>> Appraisals = new List<IAppraisal<T>>();

        public float GetScore( T context )
        {
            return this.Appraisals.Sum(t => t.GetScore(context));
        }
    }
}

