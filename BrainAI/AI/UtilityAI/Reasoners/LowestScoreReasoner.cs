namespace BrainAI.AI.UtilityAI.Reasoners
{
    using BrainAI.AI.UtilityAI.Actions;

    /// <summary>
    /// The Consideration with the lowest score is selected
    /// </summary>
    public class LowestScoreReasoner<T> : Reasoner<T>
    {
        public override IAction<T> SelectBestAction(T context)
        {
            Consideration consideration = null;
            float lowestScore = float.MaxValue;
            for( var i = 0; i < this.Considerations.Count; i++ )
            {
                var score = this.Considerations[i].Appraisal.GetScore( context );
                if( score < lowestScore )
                {
                    lowestScore = score;
                    consideration = this.Considerations[i];
                }
            }

            return consideration?.Action;
        }
    }
}

