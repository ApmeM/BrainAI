namespace BrainAI.AI.BehaviorTrees.Composites
{
    /// <summary>
    /// The sequence task is similar to an "and" operation. It will return failure as soon as one of its child tasks return failure. If a
    /// child task returns success then it will sequentially run the next task. If all child tasks return success then it will return success.
    /// </summary>
    public class Sequence<T> : Composite<T>
    {
        public Sequence( AbortTypes abortType = AbortTypes.None )
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

            // if the child failed or is still running, early return
            if( status != TaskStatus.Success )
                return status;

            this.CurrentChildIndex++;

            // if the end of the children is hit the whole sequence succeeded
            if( this.CurrentChildIndex == this.Children.Count )
            {
                // reset index for next run
                this.CurrentChildIndex = 0;
                return TaskStatus.Success;
            }

            return TaskStatus.Running;
        }

        private void HandleConditionalAborts( T context )
        {
            if( this.HasLowerPriorityConditionalAbort )
                this.UpdateLowerPriorityAbortConditional( context, TaskStatus.Success );

            if( this.AbortType.Has( AbortTypes.Self ) )
                this.UpdateSelfAbortConditional( context, TaskStatus.Success );
        }

    }
}

