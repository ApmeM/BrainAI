namespace BrainAI
{
    using BrainAI.Config;

    public static class Configuration
    {
        public static ILog Log { get; set; } = new DefaultLog();

        public static IRandom Random { get; set; } = new DefaultRandom();
    }
}