using System.Collections.Generic;

namespace BrainAI.AI.UtilityAI
{
    /// <summary>
    /// Scores by checking each Appraisal score comparing it to 0.
    /// Returns 1 if child Appraisal score were converted to 0.
    /// Returns 0 othervise.
    /// </summary>
    public class NotAppraisal<T> : IAppraisal<T>
    {
        public readonly IAppraisal<T> ChildAppraisal;

        public NotAppraisal(IAppraisal<T> apparisal)
        {
            ChildAppraisal = apparisal;
        }

        public float GetScore(T context)
        {
            if (ChildAppraisal.GetScore(context) == 0)
            {
                return 1;
            }

            return 0;
        }
    }
}

