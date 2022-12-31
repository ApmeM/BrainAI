namespace BrainAI.AI.UtilityAI
{
    using System.Collections.Generic;

    /// <summary>
    /// Returns first score that is above then threshold.
    /// </summary>
    public class FirstAppraisal<T> : IAppraisal<T>
    {
        public float Threshold;

        public readonly List<IAppraisal<T>> Appraisals = new List<IAppraisal<T>>();

        public FirstAppraisal(float threshold)
        {
            Threshold = threshold;
        }

        public FirstAppraisal(float threshold, params IAppraisal<T>[] apparisals)
        {
            Threshold = threshold;
            Appraisals.AddRange(apparisals);
        }

        public float GetScore(T context)
        {
            for (var i = 0; i < Appraisals.Count; i++)
            {
                var score = Appraisals[i].GetScore(context);
                if (score > Threshold)
                    return score;
            }

            return 0;
        }
    }
}

