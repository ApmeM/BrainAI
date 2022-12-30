namespace BrainAI.AI.UtilityAI
{
    using System.Collections.Generic;

    /// <summary>
    /// Action that contains a List of Actions that it will execute sequentially
    /// </summary>
    public class CompositeAction<T> : IAction<T>
    {
        public readonly List<IAction<T>> Actions = new List<IAction<T>>();

        public CompositeAction()
        {

        }

        public CompositeAction(params IAction<T>[] actions)
        {
            this.Actions.AddRange(actions);
        }

        public void Enter(T context)
        {
            for (var i = 0; i < this.Actions.Count; i++)
            {
                this.Actions[i].Enter(context);
            }
        }

        public void Execute(T context)
        {
            for (var i = 0; i < this.Actions.Count; i++)
            {
                this.Actions[i].Execute(context);
            }
        }

        public void Exit(T context)
        {
            for (var i = 0; i < this.Actions.Count; i++)
            {
                this.Actions[i].Exit(context);
            }
        }
    }
}

