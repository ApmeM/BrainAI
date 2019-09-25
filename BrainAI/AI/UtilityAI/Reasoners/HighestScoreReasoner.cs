namespace BrainAI.AI.UtilityAI.Reasoners
{
    using BrainAI.AI.UtilityAI.Considerations;

    /// <summary>
    /// The Consideration with the highest score is selected
    /// </summary>
    public class HighestScoreReasoner<T> : Reasoner<T>
    {
        protected override IConsideration<T> SelectBestConsideration( T context )
        {
            var highestScore = this.DefaultConsideration.GetScore( context );
            IConsideration<T> consideration = null;
            for( var i = 0; i < this.Considerations.Count; i++ )
            {
                var score = this.Considerations[i].GetScore( context );
                if( score > highestScore )
                {
                    highestScore = score;
                    consideration = this.Considerations[i];
                }
            }

            if( consideration == null )
                return this.DefaultConsideration;

            return consideration;
        }
    }
}

