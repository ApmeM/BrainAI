namespace BrainAI.UtilityAI.Actions
{
    /// <summary>
    /// Action that logs text
    /// </summary>
    public class LogAction<T> : IAction<T>
    {
        private readonly string text;


        public LogAction( string text )
        {
            this.text = text;
        }

        public void Execute( T context )
        {
            Configuration.Log.Info( this.text );
        }
    }
}

