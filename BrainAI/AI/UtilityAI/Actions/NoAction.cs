namespace BrainAI.AI.UtilityAI.Actions
{
    using System;

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

