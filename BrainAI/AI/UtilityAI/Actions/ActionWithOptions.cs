using System;
using System.Collections.Generic;


namespace BrainAI.AI.UtilityAI
{
    using BrainAI.AI.UtilityAI.Actions;

    /// <summary>
    /// Action that encompasses a List of options. The options are passed to Appraisals which score and locate the best option.
    /// </summary>
    public abstract class ActionWithOptions<TU, TV> : IAction<TU>
    {
        public readonly List<IActionOptionAppraisal<TU,TV>> Appraisals = new List<IActionOptionAppraisal<TU,TV>>();

        public TV GetBestOption( TU context, List<TV> options )
        {
            var result = default(TV);
            var bestScore = float.MinValue;

            for( var i = 0; i < options.Count; i++ )
            {
                var option = options[i];
                var current = 0f;
                for( var j = 0; j < this.Appraisals.Count; j++ )
                    current += this.Appraisals[j].GetScore( context, option );

                if( current > bestScore )
                {
                    bestScore = current;
                    result = option;
                }
            }

            return result;
        }

        public abstract void Execute( TU context );
    }
}

