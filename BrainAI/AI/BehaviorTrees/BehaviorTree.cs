namespace BrainAI.AI.BehaviorTrees
{
    using BrainAI.AI;

    /// <summary>
    /// root class used to control a BehaviorTree. Handles storing the context
    /// </summary>
    public class BehaviorTree<T> : IAITurn
    {
        /// <summary>
        /// The context should contain all the data needed to run the tree
        /// </summary>
        private readonly T context;

        /// <summary>
        /// root node of the tree
        /// </summary>
        private readonly Behavior<T> root;

        public BehaviorTree( T context, Behavior<T> rootNode )
        {
            this.context = context;
            this.root = rootNode;
        }

        public void Tick()
        {
            this.root.Tick(this.context);
        }
    }
}
