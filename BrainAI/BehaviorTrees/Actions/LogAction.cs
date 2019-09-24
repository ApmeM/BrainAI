namespace BrainAI.BehaviorTrees.Actions
{
    /// <summary>
    /// simple task which will output the specified text and return success. It can be used for debugging.
    /// </summary>
    public class LogAction<T> : Behavior<T>
    {
        /// <summary>
        /// text to log
        /// </summary>
        public string Text;

        /// <summary>
        /// is this text an error
        /// </summary>
        public bool IsError;


        public LogAction( string text )
        {
            this.Text = text;
        }


        public override TaskStatus Update( T context )
        {
            if( this.IsError )
                Configuration.Log.Error( this.Text );
            else
                Configuration.Log.Info( this.Text );

            return TaskStatus.Success;
        }
    }
}

