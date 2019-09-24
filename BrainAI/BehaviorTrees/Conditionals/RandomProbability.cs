namespace BrainAI.BehaviorTrees.Conditionals
{
    /// <summary>
    /// returns success when the random probability is above the successProbability probability. It will otherwise return failure.
    /// successProbability should be between 0 and 1.
    /// </summary>
    public class RandomProbability<T> : Behavior<T>, IConditional<T>
    {
        /// <summary>
        /// The chance that the task will return success
        /// </summary>
        private readonly int successProbability;


        public RandomProbability( int successProbability )
        {
            this.successProbability = successProbability;
        }


        public override TaskStatus Update( T context )
        {
            if( Configuration.Random.NextFloat() > this.successProbability )
                return TaskStatus.Success;
            return TaskStatus.Failure;
        }
    }
}

