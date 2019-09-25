namespace BrainAI.AI.BehaviorTrees.Actions
{
    using System;

    /// <summary>
    /// wraps a Func so that you can avoid having to subclass to create new actions
    /// </summary>
    public class ExecuteAction<T> : Behavior<T>
    {
        private readonly Func<T,TaskStatus> action;


        public ExecuteAction( Func<T,TaskStatus> action )
        {
            this.action = action;
        }


        public override TaskStatus Update( T context )
        {
            return this.action( context );
        }
    }
}

