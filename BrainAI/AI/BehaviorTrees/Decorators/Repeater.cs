namespace BrainAI.AI.BehaviorTrees.Decorators
{
    /// <summary>
    /// will repeat execution of its child task until the child task has been run a specified number of times. It has the option of
    /// continuing to execute the child task even if the child task returns a failure.
    /// </summary>
    public class Repeater<T> : Decorator<T>
    {
        /// <summary>
        /// The number of times to repeat the execution of its child task
        /// </summary>
        public int Count;

        /// <summary>
        /// Allows the repeater to repeat forever
        /// </summary>
        public bool RepeatForever;

        /// <summary>
        /// Should the task return if the child task returns a failure
        /// </summary>
        public bool EndOnFailure;

        private int iterationCount;


        public Repeater( int count, bool endOnFailure = false )
        {
            this.Count = count;
            this.EndOnFailure = endOnFailure;
        }


        public Repeater( bool repeatForever, bool endOnFailure = false )
        {
            this.RepeatForever = repeatForever;
            this.EndOnFailure = endOnFailure;
        }


        public override void OnStart()
        {
            this.iterationCount = 0;
        }
    

        public override TaskStatus Update( T context )
        {
            // early out if we are done. we check here and after running just in case the count is 0
            if( !this.RepeatForever && this.iterationCount == this.Count )
                return TaskStatus.Success;
            
            var status = this.Child.Tick( context );
            this.iterationCount++;

            if( this.EndOnFailure && status == TaskStatus.Failure )
                return TaskStatus.Success;

            if( !this.RepeatForever && this.iterationCount == this.Count )
                return TaskStatus.Success;

            return TaskStatus.Running;
        }
    }
}

