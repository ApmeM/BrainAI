namespace BrainAI.AI.BehaviorTrees.Composites
{
    /// <summary>
    /// the parallel task will run each child task until a child task returns failure. The difference is that the parallel task will run all of
    /// its children tasks simultaneously versus running each task one at a time. Like the sequence class, the parallel task will return
    /// success once all of its children tasks have returned success. If one tasks returns failure the parallel task will end all of the child
    /// tasks and return failure.
    /// </summary>
    public class ParallelSequence<T> : Composite<T>
    {
        public override TaskStatus Update( T context )
        {
            var didAllSucceed = true;
            for( var i = 0; i < this.Children.Count; i++ )
            {
                var child = this.Children[i];
                child.Tick( context );

                // if any child fails the whole branch fails
                if( child.Status == TaskStatus.Failure )
                    return TaskStatus.Failure;
                // if all children didn't succeed, we're not done yet
                else if( child.Status != TaskStatus.Success )
                    didAllSucceed = false;
            }

            if( didAllSucceed )
                return TaskStatus.Success;

            return TaskStatus.Running;
        }
    }
}

