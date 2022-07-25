namespace BrainAI.AI.UtilityAI.Appraisals
{
    using System.Collections.Generic;

    /// <summary>
    /// Scores by summing child Appraisals until a child scores below the threshold
    /// </summary>
    public class FirstAfterThresholdAppraisal<T> : IAppraisal<T>
    {
        public float Threshold;

        public readonly List<IAppraisal<T>> Appraisals = new List<IAppraisal<T>>();

        public FirstAfterThresholdAppraisal(float threshold)
        {
            Threshold = threshold;
        }

        public float GetScore(T context)
        {
            var sum = 0f;
            for (var i = 0; i < Appraisals.Count; i++)
            {
                var score = Appraisals[i].GetScore(context);
                if (score < Threshold)
                    return sum;
                sum += score;
            }

            return sum;
        }
    }
}

