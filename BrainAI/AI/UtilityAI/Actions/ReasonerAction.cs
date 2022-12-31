using System;

namespace BrainAI.AI.UtilityAI
{
    /// <summary>
    /// Action that calls through to another Reasoner.
    /// lastActionGetter and lastActionSetter helps to maintain enter and exit methods.
    /// if unset or set in improper way - enter and exit methods might be called randomly.
    /// for example usage <see cref="BrainAI.AI.UtilityAI.UtilityAI" />
    /// </summary>
    public class ReasonerAction<T> : IAction<T>
    {
        private readonly Reasoner<T> reasoner;
        private readonly Action<IAction<T>> lastActionSetter;
        private readonly Func<IAction<T>> lastActionGetter;

        public ReasonerAction(Reasoner<T> reasoner)
        {
            this.reasoner = reasoner;
        }

        public ReasonerAction(Reasoner<T> reasoner, Action<IAction<T>> lastActionSetter, Func<IAction<T>> lastActionGetter)
        {
            this.reasoner = reasoner;
            this.lastActionSetter = lastActionSetter;
            this.lastActionGetter = lastActionGetter;
        }

        public void Enter(T context)
        {
        }

        public void Execute(T context)
        {
            var action = this.reasoner.SelectBestAction(context);
            var lastAction = lastActionGetter?.Invoke();
            if (lastAction != action)
            {
                lastAction?.Exit(context);
                action?.Enter(context);
            }
            action?.Execute(context);
            lastActionSetter?.Invoke(action);
        }

        public void Exit(T context)
        {
        }
    }
}

