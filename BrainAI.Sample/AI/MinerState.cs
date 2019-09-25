namespace BrainAI.Sample.AI
{
    public class MinerState
    {
        public enum Location
        {
            InTransit,
            Bank,
            Mine,
            Home,
            Saloon
        }

        public const int MAX_FATIGUE = 10;
        public const int MAX_GOLD = 8;
        public const int MAX_THIRST = 5;

        public int Fatigue;
        public int Thirst;
        public int Gold;
        public int GoldInBank;

        public Location CurrentLocation = Location.Home;
    }
}