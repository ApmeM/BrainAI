namespace BrainAI.AI.UtilityAI.Actions
{
    using System;

    /// <summary>
    /// Wraps a dotnet Action for use as an IAction without having to create a new class
    /// </summary>
    public class ActionAction<T> : IAction<T>
    {
        private readonly Action<T> action;


        public ActionAction( Action<T> action )
        {
            this.action = action;
        }

        public void Execute( T context )
        {
            this.action( context );
        }
    }
}

