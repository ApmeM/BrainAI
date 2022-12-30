namespace BrainAI.AI.BehaviorTrees
{
    /// <summary>
    /// will always return failure except when the child task is running
    /// </summary>
    public class AlwaysFailDecorator<T> : Decorator<T>
    {
        public override TaskStatus Update( T context )
        {
            var status = this.Child.Update( context );

            if( status == TaskStatus.Running )
                return TaskStatus.Running;
            
            return TaskStatus.Failure;
        }
    }
}

