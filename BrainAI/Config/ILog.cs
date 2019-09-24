namespace BrainAI.Config
{
    public interface ILog
    {
        void Error(string text);

        void Info(string text);
    }
}