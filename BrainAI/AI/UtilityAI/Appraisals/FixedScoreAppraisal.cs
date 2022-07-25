namespace BrainAI.AI.UtilityAI.Appraisals
{
    /// <summary>
    /// always returns a fixed score. Serves double duty as a default Consideration.
    /// </summary>
    public class FixedScoreAppraisal<T> : IAppraisal<T>
    {
        public float Score;

        public FixedScoreAppraisal(float score)
        {
            Score = score;
        }

        public float GetScore(T context)
        {
            return Score;
        }
    }
}

