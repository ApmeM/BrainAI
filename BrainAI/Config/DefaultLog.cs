namespace BrainAI.Config
{
    public class DefaultLog : ILog
    {
        public void Error(string text)
        {
            System.Diagnostics.Debug.WriteLine("ERROR: " + text);
        }

        public void Info(string text)
        {
            System.Diagnostics.Debug.WriteLine("INFO: " + text);
        }
    }
}