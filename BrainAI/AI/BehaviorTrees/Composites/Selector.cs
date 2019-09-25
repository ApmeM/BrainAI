namespace BrainAI.AI.BehaviorTrees.Composites
{
    /// <summary>
    /// The selector task is similar to an "or" operation. It will return success as soon as one of its child tasks return success. If a
    /// child task returns failure then it will sequentially run the next task. If no child task returns success then it will return failure.
    /// </summary>
    public class Selector<T> : Composite<T>
    {
        public Selector( AbortTypes abortType = AbortTypes.None )
        {
            this.AbortType = abortType;
        }


        public override TaskStatus Update( T context )
        {
            // first, we handle conditional aborts if we are not already on the first child
            if( this.CurrentChildIndex != 0 )
                this.HandleConditionalAborts( context );
            
            var current = this.Children[this.CurrentChildIndex];
            var status = current.Tick( context );

            // if the child succeeds or is still running, early return.
            if( status != TaskStatus.Failure )
                return status;
            
            this.CurrentChildIndex++;

            // if the end of the children is hit, that means the whole thing fails.
            if( this.CurrentChildIndex == this.Children.Count )
            {
                // reset index otherwise it will crash on next run through
                this.CurrentChildIndex = 0;
                return TaskStatus.Failure;
            }

            return TaskStatus.Running;
        }

        private void HandleConditionalAborts( T context )
        {
            // check any lower priority tasks to see if they changed to a success
            if( this.HasLowerPriorityConditionalAbort )
                this.UpdateLowerPriorityAbortConditional( context, TaskStatus.Failure );

            if( this.AbortType.Has( AbortTypes.Self ) )
                this.UpdateSelfAbortConditional( context, TaskStatus.Failure );
        }
    }
}

