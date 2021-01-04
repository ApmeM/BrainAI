namespace BrainAI.InfluenceMap.Fading
{
    using BrainAI.Pathfinding;

    public interface IFading
    {
        Point GetForce(Point vector, float chargeValue);
        float GetPower(float distance, float chargeValue);
    }
}