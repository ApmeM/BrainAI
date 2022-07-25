namespace BrainAI.AI.UtilityAI.Appraisals
{
    using System.Collections.Generic;

    /// <summary>
    /// Scores by summing the score of all child Appraisals
    /// </summary>
    public class SumOfChildrenAppraisal<T> : IAppraisal<T>
    {
        public readonly List<IAppraisal<T>> Appraisals = new List<IAppraisal<T>>();

        public SumOfChildrenAppraisal()
        {

        }

        public SumOfChildrenAppraisal(params IAppraisal<T>[] apparisals)
        {
            Appraisals.AddRange(apparisals);
        }

        public float GetScore(T context)
        {
            var result = 0f;
            for (var i = 0; i < Appraisals.Count; i++)
            {
                result += Appraisals[i].GetScore(context);
            }
            return result;
        }
    }
}

