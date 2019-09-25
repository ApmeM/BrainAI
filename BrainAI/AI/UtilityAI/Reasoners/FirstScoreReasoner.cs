namespace BrainAI.AI.UtilityAI.Reasoners
{
    using BrainAI.AI.UtilityAI.Considerations;

    /// <summary>
    /// The first Consideration to score above the score of the Default Consideration is selected
    /// </summary>
    public class FirstScoreReasoner<T> : Reasoner<T>
    {
        protected override IConsideration<T> SelectBestConsideration( T context )
        {
            var defaultScore = this.DefaultConsideration.GetScore( context );
            for( var i = 0; i < this.Considerations.Count; i++ )
            {
                if( this.Considerations[i].GetScore( context ) >= defaultScore )
                    return this.Considerations[i];
            }

            return this.DefaultConsideration;
        }
    }
}

