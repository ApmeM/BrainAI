namespace BrainAI.AI.UtilityAI.Reasoners
{
    using BrainAI.AI.UtilityAI.Actions;

    /// <summary>
    /// The Consideration with the highest score is selected
    /// </summary>
    public class HighestScoreReasoner<T> : Reasoner<T>
    {
        public override IAction<T> SelectBestAction(T context)
        {
            Consideration consideration = null;
            float highestScore = float.MinValue;
            for( var i = 0; i < this.Considerations.Count; i++ )
            {
                var score = this.Considerations[i].Appraisal.GetScore( context );
                if( score > highestScore )
                {
                    highestScore = score;
                    consideration = this.Considerations[i];
                }
            }

            return consideration?.Action;
        }
    }
}

