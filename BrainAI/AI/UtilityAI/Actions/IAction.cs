namespace BrainAI.AI.UtilityAI
{
    public interface IAction<T>
    {
        void Enter(T context);
        void Execute(T context);
        void Exit(T context);
    }
}

