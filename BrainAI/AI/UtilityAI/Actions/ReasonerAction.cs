using System;

namespace BrainAI.AI.UtilityAI
{
    /// <summary>
    /// Action that calls through to another Reasoner
    /// </summary>
    public class ReasonerAction<T> : IAction<T>
    {
        private readonly Reasoner<T> reasoner;
        private IAction<T> lastAction;

        public ReasonerAction(Reasoner<T> reasoner)
        {
            this.reasoner = reasoner;
        }

        public void Enter(T context)
        {
        }

        public void Execute(T context)
        {
            var action = this.reasoner.SelectBestAction(context);
            if (lastAction != action)
            {
                lastAction?.Exit(context);
                action?.Enter(context);
            }
            action?.Execute(context);
            lastAction = action;
        }

        public void Exit(T context)
        {
        }
    }
}

