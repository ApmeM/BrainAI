using System.Collections.Generic;
using System.Linq;

namespace BrainAI.AI.UtilityAI
{
    public class CompositeIntent<T> : IIntent<T>
    {
        private readonly List<IIntent<T>> intents;

        public CompositeIntent(params IIntent<T>[] intents)
        {
            this.intents = new List<IIntent<T>>(intents);
        }

        public void Enter(T context)
        {
            this.intents.ForEach(a => a.Enter(context));
        }

        public bool Execute(T context)
        {
            this.intents.RemoveAll(a => a.Execute(context));
            return !this.intents.Any();
        }

        public void Exit(T context)
        {
            this.intents.ForEach(a => a.Exit(context));
        }
    }
}