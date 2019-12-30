namespace BrainAI.InfluenceMap.Fading
{
    using BrainAI.Pathfinding;

    public interface IFading
    {
        Point GetForce(Point vector, float chargeValue);
    }
}