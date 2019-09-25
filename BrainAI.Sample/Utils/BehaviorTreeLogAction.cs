namespace BrainAI.Sample.Utils
{
    using System;

    using BrainAI.AI.BehaviorTrees;

    /// <summary>
    /// simple task which will output the specified text and return success. It can be used for debugging.
    /// </summary>
    public class BehaviorTreeLogAction<T> : Behavior<T>
    {
        /// <summary>
        /// text to log
        /// </summary>
        public string Text;

        /// <summary>
        /// is this text an error
        /// </summary>
        public bool IsError;


        public BehaviorTreeLogAction( string text )
        {
            this.Text = text;
        }


        public override TaskStatus Update( T context )
        {
            if (this.IsError)
                Console.WriteLine($"ERROR: {this.Text}");
            else
                Console.WriteLine($"INFO: {this.Text}");

            return TaskStatus.Success;
        }
    }
}

