namespace BrainAI.Sample.Utils
{
    using BrainAI.AI.BehaviorTrees;

    public static class BehaviorTreeBuilderExtension
    {
        public static BehaviorTreeBuilder<T> LogAction<T>(this BehaviorTreeBuilder<T> builder, string text)
        {
            return builder.AddChildBehavior(new BehaviorTreeLogAction<T>(text));
        }
    }
}