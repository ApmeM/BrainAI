namespace BrainAI.AI.UtilityAI
{
    using System;

    /// <summary>
    /// Wraps a dotnet Action for use as an IAction without having to create a new class
    /// </summary>
    public class ActionAction<T> : IAction<T>
    {
        private readonly Action<T> enter;
        private readonly Action<T> action;
        private readonly Action<T> exit;

        public ActionAction(Action<T> action)
        {
            this.action = action;
        }

        public ActionAction(Action<T> enter, Action<T> action, Action<T> exit)
        {
            this.enter = enter;
            this.action = action;
            this.exit = exit;
        }

        public void Enter(T context)
        {
            this.enter?.Invoke(context);
        }

        public void Execute(T context)
        {
            this.action(context);
        }

        public void Exit(T context)
        {
            this.exit?.Invoke(context);
        }
    }
}

