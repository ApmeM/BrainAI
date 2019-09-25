namespace BrainAI.AI.BehaviorTrees.Actions
{
    /// <summary>
    /// runs an entire BehaviorTree as a child and returns success
    /// </summary>
    public class BehaviorTreeReference<T> : Behavior<T>
    {
        private readonly BehaviorTree<T> childTree;


        public BehaviorTreeReference( BehaviorTree<T> tree )
        {
            this.childTree = tree;
        }


        public override TaskStatus Update( T context )
        {
            this.childTree.Tick();
            return TaskStatus.Success;
        }
    }
}

