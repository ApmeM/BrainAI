namespace BrainAI.AI.UtilityAI
{
    public interface IIntent<T>
    {
        void Enter(T context);
        bool Execute(T context);
        void Exit(T context);
    }
}