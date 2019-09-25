namespace BrainAI.AI.BehaviorTrees.Decorators
{
    /// <summary>
    /// will keep executing its child task until the child task returns success
    /// </summary>
    public class UntilSuccess<T> : Decorator<T>
    {
        public override TaskStatus Update( T context )
        {
            var status = this.Child.Tick( context );

            if( status != TaskStatus.Success )
                return TaskStatus.Running;

            return TaskStatus.Success;
        }
    }
}

