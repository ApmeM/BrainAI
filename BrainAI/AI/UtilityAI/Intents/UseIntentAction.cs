namespace BrainAI.AI.UtilityAI
{
    public class UseIntentAction<T> : IAction<T> where T : IIntentContainer<T>
    {
        public void Enter(T context)
        {
            context.Intent.Enter(context);
        }

        public void Execute(T context)
        {
            var isFinished = context.Intent.Execute(context);
            if (isFinished)
            {
                this.Exit(context);
            }
        }

        public void Exit(T context)
        {
            context.Intent?.Exit(context);
            context.Intent = null;
        }
    }
}