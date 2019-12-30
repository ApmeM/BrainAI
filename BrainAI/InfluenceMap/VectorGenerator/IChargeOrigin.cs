namespace BrainAI.InfluenceMap.VectorGenerator
{
    using BrainAI.Pathfinding;

    public interface IChargeOrigin
    {
        Point GetVector(Point fromPoint);
    }
}