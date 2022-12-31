namespace BrainAI.AI.UtilityAI
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Return max score of child appraisals.
    /// For binary child appraisals (that returns 1 or 0) can be used as boolean 'OR' operator.
    /// </summary>
    public class MaxAppraisal<T> : IAppraisal<T>
    {
        public readonly List<IAppraisal<T>> Appraisals = new List<IAppraisal<T>>();

        public MaxAppraisal()
        {
        }

        public MaxAppraisal(params IAppraisal<T>[] apparisals)
        {
            Appraisals.AddRange(apparisals);
        }

        public float GetScore(T context)
        {
            if (Appraisals.Count == 0)
            {
                return 0;
            }

            var max = float.MinValue;
            for (var i = 0; i < Appraisals.Count; i++)
            {
                var score = Appraisals[i].GetScore(context);
                max = Math.Max(max, score);
            }

            return max;
        }
    }
}

