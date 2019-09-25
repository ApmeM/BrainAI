namespace BrainAI.AI.BehaviorTrees.Decorators
{
    /// <summary>
    /// will always return success except when the child task is running
    /// </summary>
    public class AlwaysSucceed<T> : Decorator<T>
    {
        public override TaskStatus Update( T context )
        {
            var status = this.Child.Update( context );

            if( status == TaskStatus.Running )
                return TaskStatus.Running;

            return TaskStatus.Success;
        }
    }
}

