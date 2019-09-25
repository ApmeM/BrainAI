namespace BrainAI.AI.UtilityAI.Actions
{
    using System;

    /// <summary>
    /// wraps an Action for use as an IAction without having to create a new class
    /// </summary>
    public class ActionExecutor<T> : IAction<T>
    {
        private readonly Action<T> action;


        public ActionExecutor( Action<T> action )
        {
            this.action = action;
        }


        public void Execute( T context )
        {
            this.action( context );
        }
    }
}

