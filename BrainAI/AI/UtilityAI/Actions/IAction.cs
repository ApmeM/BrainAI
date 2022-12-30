namespace BrainAI.AI.UtilityAI
{
    public interface IAction<T>
    {
        void Execute( T context );
    }
}

