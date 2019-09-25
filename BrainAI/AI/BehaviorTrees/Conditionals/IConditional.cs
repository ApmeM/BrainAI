namespace BrainAI.AI.BehaviorTrees.Conditionals
{
    /// <summary>
    /// interface used just to identify if a Behavior is a conditional. it will always be applied to a Behavior which already has the update method.
    /// </summary>
    public interface IConditional<in T>
    {
        TaskStatus Update( T context );
    }
}
