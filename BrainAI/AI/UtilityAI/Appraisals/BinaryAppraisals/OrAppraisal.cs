using System.Collections.Generic;

namespace BrainAI.AI.UtilityAI
{
    /// <summary>
    /// Scores by checking each Appraisal score comparing it to 0.
    /// Returns 1 if any Appraisal scores were converted to 1.
    /// Returns 0 othervise or if list of Appraisals is empty.
    /// </summary>
    public class OrAppraisal<T> : IAppraisal<T>
    {
        public readonly List<IAppraisal<T>> Appraisals = new List<IAppraisal<T>>();


        public OrAppraisal()
        {

        }

        public OrAppraisal(params IAppraisal<T>[] apparisals)
        {
            Appraisals.AddRange(apparisals);
        }

        public float GetScore(T context)
        {
            if (Appraisals.Count == 0)
            {
                return 0;
            }

            for (var i = 0; i < Appraisals.Count; i++)
            {
                if (Appraisals[i].GetScore(context) != 0)
                {
                    return 1;
                }
            }

            return 0;
        }
    }
}

