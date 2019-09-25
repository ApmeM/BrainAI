namespace BrainAI.AI.UtilityAI.Considerations
{
    using System.Collections.Generic;

    using BrainAI.AI.UtilityAI.Actions;
    using BrainAI.AI.UtilityAI.Considerations.Appraisals;

    /// <summary>
    /// Scores by summing child Appraisals until a child scores below the threshold
    /// </summary>
    public class ThresholdConsideration<T> : IConsideration<T>
    {
        public float Threshold;

        public IAction<T> Action { get; set; }

        public readonly List<IAppraisal<T>> Appraisals = new List<IAppraisal<T>>();

        public ThresholdConsideration( float threshold )
        {
            this.Threshold = threshold;
        }

        public float GetScore( T context )
        {
            var sum = 0f;
            for( var i = 0; i < this.Appraisals.Count; i++ )
            {
                var score = this.Appraisals[i].GetScore( context );
                if( score < this.Threshold )
                    return sum;
                sum += score;
            }

            return sum;
        }
    }
}

