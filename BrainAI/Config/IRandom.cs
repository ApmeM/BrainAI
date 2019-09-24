namespace BrainAI.Config
{
    public interface IRandom
    {
        float NextFloat();

        int Range(int from, int to);
    }
}