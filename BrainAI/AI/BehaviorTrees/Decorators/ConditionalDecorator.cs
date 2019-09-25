namespace BrainAI.AI.BehaviorTrees.Decorators
{
    using BrainAI.AI.BehaviorTrees.Conditionals;

    /// <summary>
    /// decorator that will only run its child if a condition is met. By default, the condition will be reevaluated every tick.
    /// </summary>
    public class ConditionalDecorator<T> : Decorator<T>, IConditional<T>
    {
        private readonly IConditional<T> conditional;
        private readonly bool shouldReevaluate;
        private TaskStatus conditionalStatus;

        public ConditionalDecorator( IConditional<T> conditional, bool shouldReevalute )
        {
            this.conditional = conditional;
            this.shouldReevaluate = shouldReevalute;
        }


        public ConditionalDecorator( IConditional<T> conditional ) : this( conditional, true )
        {}


        public override void Invalidate()
        {
            base.Invalidate();
            this.conditionalStatus = TaskStatus.Invalid;
        }


        public override void OnStart()
        {
            this.conditionalStatus = TaskStatus.Invalid;
        }

        
        public override TaskStatus Update( T context )
        {
            // evalute the condition if we need to
            this.conditionalStatus = this.ExecuteConditional( context );
            
            if( this.conditionalStatus == TaskStatus.Success )
                return this.Child.Tick( context );

            return TaskStatus.Failure;
        }


        /// <summary>
        /// executes the conditional either following the shouldReevaluate flag or with an option to force an update. Aborts will force the
        /// update to make sure they get the proper data if a Conditional changes.
        /// </summary>
        /// <returns>The conditional.</returns>
        /// <param name="context">Context.</param>
        /// <param name="forceUpdate">If set to <c>true</c> force update.</param>
        internal TaskStatus ExecuteConditional( T context, bool forceUpdate = false )
        {
            if( forceUpdate || this.shouldReevaluate || this.conditionalStatus == TaskStatus.Invalid )
                this.conditionalStatus = this.conditional.Update( context );
            return this.conditionalStatus;
        }

    }
}

