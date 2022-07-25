using System.Collections.Generic;

namespace BrainAI.AI.UtilityAI.Appraisals
{
    /// <summary>
    /// Scores by summing the score of all child Appraisals
    /// </summary>
    public class MultiplyOfchildrenAppraisal<T> : IAppraisal<T>
    {
        public readonly List<IAppraisal<T>> Appraisals = new List<IAppraisal<T>>();


        public MultiplyOfchildrenAppraisal()
        {

        }

        public MultiplyOfchildrenAppraisal(params IAppraisal<T>[] apparisals)
        {
            Appraisals.AddRange(apparisals);
        }

        public float GetScore(T context)
        {
            var result = 1f;
            for (var i = 0; i < Appraisals.Count; i++)
            {
                result *= Appraisals[i].GetScore(context);
            }
            return result;
        }
    }
}

