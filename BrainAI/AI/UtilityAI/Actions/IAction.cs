namespace BrainAI.AI.UtilityAI.Actions
{
    public interface IAction<T>
    {
        void Execute( T context );
    }
}

