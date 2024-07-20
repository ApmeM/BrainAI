namespace BrainAI.Pathfinding
{
    public abstract class CoveragePathfinder<T> : Pathfinder<T>, ICoveragePathfinder<T>
    {
        public void Search(T start, int additionalDepth)
        {
            this.PrepareSearch();
            this.StartNewSearch(start);

            InternalSearch(additionalDepth);
        }
    }
}