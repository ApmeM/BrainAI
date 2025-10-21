using System;

namespace BrainAI.AI.UtilityAI
{
    public class ActionIntent<T> : IIntent<T>
    {
        private readonly Action<T> enter;
        private readonly Func<T, bool> action;
        private readonly Action<T> exit;

        public ActionIntent(Func<T, bool> action)
        {
            this.action = action;
        }

        public ActionIntent(Action<T> enter, Func<T, bool> action, Action<T> exit)
        {
            this.enter = enter;
            this.action = action;
            this.exit = exit;
        }

        public void Enter(T context)
        {
            this.enter?.Invoke(context);
        }

        public bool Execute(T context)
        {
            return this.action(context);
        }

        public void Exit(T context)
        {
            this.exit?.Invoke(context);
        }
    }
}