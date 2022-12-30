namespace BrainAI.InfluenceMap
{
    using BrainAI.Pathfinding;

    public interface IChargeOrigin
    {
        Point GetVector(Point fromPoint);
    }
}