using System;

namespace BrainAI.AI.UtilityAI
{
    public class SetIntentAction<T, T1> : IAction<T> where T : IIntentContainer<T>
                                                     where T1 : IIntent<T>
    {
        private readonly Func<T, T1> intentFactory;

        public SetIntentAction(T1 intent)
        {
            this.intentFactory = ctx => intent;
        }

        public SetIntentAction(Func<T, T1> intentFactory)
        {
            this.intentFactory = intentFactory;
        }

        public void Enter(T context)
        {
        }

        public void Execute(T context)
        {
            context.Intent = this.intentFactory(context);
        }

        public void Exit(T context)
        {
        }
    }
}