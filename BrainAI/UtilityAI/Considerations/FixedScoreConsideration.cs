namespace BrainAI.UtilityAI.Considerations
{
    using BrainAI.UtilityAI.Actions;

    /// <summary>
    /// always returns a fixed score. Serves double duty as a default Consideration.
    /// </summary>
    public class FixedScoreConsideration<T> : IConsideration<T>
    {
        public float Score;

        public IAction<T> Action { get; set; }

        public FixedScoreConsideration( float score = 1 )
        {
            this.Score = score;
        }

        public float GetScore( T context )
        {
            return this.Score;
        }
    }
}

