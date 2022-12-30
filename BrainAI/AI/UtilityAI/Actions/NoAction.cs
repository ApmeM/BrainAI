namespace BrainAI.AI.UtilityAI
{
    /// <summary>
    /// No action to excute
    /// </summary>
    public class NoAction<T> : IAction<T>
    {
        public void Enter(T context)
        {
        }

        public void Execute( T context )
        {
        }

        public void Exit(T context)
        {
        }
    }
}

