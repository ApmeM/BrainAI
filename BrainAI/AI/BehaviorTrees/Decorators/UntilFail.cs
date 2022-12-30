namespace BrainAI.AI.BehaviorTrees
{
    /// <summary>
    /// will keep executing its child task until the child task returns failure
    /// </summary>
    public class UntilFailDecorator<T> : Decorator<T>
    {
        public override TaskStatus Update( T context )
        {
            var status = this.Child.Update( context );

            if( status != TaskStatus.Failure )
                return TaskStatus.Running;

            return TaskStatus.Success;
        }
    }
}

