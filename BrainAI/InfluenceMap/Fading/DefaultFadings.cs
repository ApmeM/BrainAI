namespace BrainAI.InfluenceMap.Fading
{
    public static class DefaultFadings
    {
        public static readonly IFading NoDistanceFading = new NoDistanceFading();
        public static readonly IFading LinearDistanceFading = new LinearDistanceFading(1);
        public static readonly IFading DistanceFading = new NPowDistanceFading(1);
        public static readonly IFading QuadDistanceFading = new NPowDistanceFading(2);
        public static readonly IFading TripleDistanceFading = new NPowDistanceFading(3);
    }
}
