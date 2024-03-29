﻿namespace BrainAI.AI.UtilityAI
{
    using System;

    /// <summary>
    /// Func wrapper for use as an Appraisal without having to create a subclass.
    /// </summary>
    public class ActionAppraisal<T> : IAppraisal<T>
    {
        private readonly Func<T, float> appraisalAction;

        public ActionAppraisal(Func<T, float> appraisalAction)
        {
            this.appraisalAction = appraisalAction;
        }

        public float GetScore(T context)
        {
            return appraisalAction(context);
        }
    }
}

