using System.Collections.Generic;

namespace BrainAI.AI.UtilityAI
{
    /// <summary>
    /// Scores by multiplying the score of all child Appraisals.
    /// For binary child appraisals (that returns 1 or 0) can be used as boolean 'AND' operator if threshold is set to 0.
    /// </summary>
    public class MultAppraisal<T> : IAppraisal<T>
    {
        public readonly List<IAppraisal<T>> Appraisals = new List<IAppraisal<T>>();


        public MultAppraisal()
        {

        }

        public MultAppraisal(params IAppraisal<T>[] apparisals)
        {
            Appraisals.AddRange(apparisals);
        }

        public float GetScore(T context)
        {
            if (Appraisals.Count == 0)
            {
                return 0;
            }

            var result = 1f;
            for (var i = 0; i < Appraisals.Count; i++)
            {
                if (result == 0)
                {
                    return 0;
                }
                
                result *= Appraisals[i].GetScore(context);
            }
            return result;
        }
    }
}

