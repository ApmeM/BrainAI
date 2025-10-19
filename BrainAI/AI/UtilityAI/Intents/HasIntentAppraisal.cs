namespace BrainAI.AI.UtilityAI
{
    public class HasIntentAppraisal<T> : IAppraisal<T> where T : IIntentContainer<T>
    {
        private readonly float score;

        public HasIntentAppraisal(float score)
        {
            this.score = score;
        }

        public float GetScore(T context)
        {
            return context.Intent == null ? 0 : score;
        }
    }
}