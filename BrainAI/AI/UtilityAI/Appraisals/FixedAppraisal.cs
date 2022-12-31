namespace BrainAI.AI.UtilityAI
{
    /// <summary>
    /// Always returns a fixed score.
    /// </summary>
    public class FixedAppraisal<T> : IAppraisal<T>
    {
        public float Score;

        public FixedAppraisal(float score)
        {
            Score = score;
        }

        public float GetScore(T context)
        {
            return Score;
        }
    }
}

