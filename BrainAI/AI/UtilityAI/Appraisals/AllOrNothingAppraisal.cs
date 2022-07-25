namespace BrainAI.AI.UtilityAI.Appraisals
{
    using System.Collections.Generic;

    /// <summary>
    /// Only scores if all child Appraisals score above the threshold
    /// </summary>
    public class AllOrNothingAppraisal<T> : IAppraisal<T>
    {
        public float Threshold;

        public readonly List<IAppraisal<T>> Appraisals = new List<IAppraisal<T>>();


        public AllOrNothingAppraisal(float threshold, params IAppraisal<T>[] apparisals)
        {
            Threshold = threshold;
            Appraisals.AddRange(apparisals);
        }

        public float GetScore(T context)
        {
            var sum = 0f;
            for (var i = 0; i < Appraisals.Count; i++)
            {
                var score = Appraisals[i].GetScore(context);
                if (score < Threshold)
                    return 0;
                sum += score;
            }

            return sum;
        }
    }
}

