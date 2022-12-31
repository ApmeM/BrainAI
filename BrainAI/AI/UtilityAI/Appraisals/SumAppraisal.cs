namespace BrainAI.AI.UtilityAI
{
    using System.Collections.Generic;

    /// <summary>
    /// Scores if all child Appraisals score above the threshold.
    /// If threshold not specified - float.MinValue is used.
    /// </summary>
    public class SumAppraisal<T> : IAppraisal<T>
    {
        public readonly List<IAppraisal<T>> Appraisals = new List<IAppraisal<T>>();

        public SumAppraisal()
        {
        }

        public SumAppraisal(params IAppraisal<T>[] apparisals)
        {
            Appraisals.AddRange(apparisals);
        }

        public float GetScore(T context)
        {
            var sum = 0f;
            for (var i = 0; i < Appraisals.Count; i++)
            {
                sum += Appraisals[i].GetScore(context);
            }

            return sum;
        }
    }
}

