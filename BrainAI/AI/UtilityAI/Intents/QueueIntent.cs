using System.Collections.Generic;

namespace BrainAI.AI.UtilityAI
{
    public class QueueIntent<T> : IIntent<T>
    {
        private readonly Queue<IIntent<T>> intents;
        private IIntent<T> currentIntent;

        public QueueIntent(params IIntent<T>[] intents)
        {
            this.intents = new Queue<IIntent<T>>(intents);
        }

        public void Enter(T context)
        {
            currentIntent?.Enter(context);
        }

        public bool Execute(T context)
        {
            if (currentIntent == null)
            {
                if (intents.Count == 0)
                {
                    return true;
                }
                currentIntent = intents.Dequeue();
                currentIntent.Enter(context);
            }

            var deleted = currentIntent.Execute(context);
            if (deleted)
            {
                currentIntent.Exit(context);
                currentIntent = null;
            }

            return false;
        }

        public void Exit(T context)
        {
            currentIntent?.Exit(context);
        }
    }
}