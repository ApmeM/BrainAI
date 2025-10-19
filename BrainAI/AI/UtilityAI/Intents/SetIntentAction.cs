namespace BrainAI.AI.UtilityAI
{
    public class SetIntentAction<T, T1> : IAction<T> where T : IIntentContainer<T>
                                                     where T1 : IIntent<T>
    {
        private readonly T1 intent;

        public SetIntentAction(T1 intent)
        {
            this.intent = intent;
        }

        public void Enter(T context)
        {
        }

        public void Execute(T context)
        {
            context.Intent = intent;
        }

        public void Exit(T context)
        {
        }
    }
}