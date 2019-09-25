namespace BrainAI.AI.UtilityAI.Considerations.Appraisals
{
    using System;

    /// <summary>
    /// wraps a Func for use as an Appraisal without having to create a subclass
    /// </summary>
    public class ActionAppraisal<T> : IAppraisal<T>
    {
        private readonly Func<T,float> appraisalAction;

        public ActionAppraisal( Func<T,float> appraisalAction )
        {
            this.appraisalAction = appraisalAction;
        }

        public float GetScore( T context )
        {
            return this.appraisalAction( context );
        }
    }
}

