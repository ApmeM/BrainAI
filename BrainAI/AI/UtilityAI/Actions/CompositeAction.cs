namespace BrainAI.AI.UtilityAI.Actions
{
    using System.Collections.Generic;

    /// <summary>
    /// Action that contains a List of Actions that it will execute sequentially
    /// </summary>
    public class CompositeAction<T> : IAction<T>
    {
        public readonly List<IAction<T>> Actions = new List<IAction<T>>();

        public void Execute( T context )
        {
            for (var i = 0; i < this.Actions.Count; i++)
            {
                this.Actions[i].Execute( context );
            }
        }
    }
}

