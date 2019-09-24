namespace BrainAI.UtilityAI.Actions
{
    public interface IAction<T>
    {
        void Execute( T context );
    }
}

