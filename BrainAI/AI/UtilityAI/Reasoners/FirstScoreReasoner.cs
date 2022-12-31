namespace BrainAI.AI.UtilityAI
{
    /// <summary>
    /// Selects first action with score above the threshold
    /// </summary>
    public class FirstScoreReasoner<T> : Reasoner<T>
    {
        private readonly float threshold;

        public FirstScoreReasoner(float threshold)
        {
            this.threshold = threshold;
        }

        public override IAction<T> SelectBestAction(T context)
        {
            for( var i = 0; i < this.Considerations.Count; i++ )
            {
                var score = this.Considerations[i].Appraisal.GetScore( context );
                if( score >= threshold )
                {
                    return this.Considerations[i].Action;
                }
            }

            return null;
        }
    }
}

