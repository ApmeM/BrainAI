namespace BrainAI.AI.UtilityAI
{
    /// <summary>
    /// No action to excute
    /// </summary>
    public class NoAction<T> : IAction<T>
    {
        public void Execute( T context )
        {
        }
    }
}

