namespace BrainAI.AI.UtilityAI.Actions
{
    /// <summary>
    /// Appraisal for use with an ActionWithOptions
    /// </summary>
    public interface IActionOptionAppraisal<in TU, in TV>
    {
        float GetScore( TU context, TV option );
    }
}

