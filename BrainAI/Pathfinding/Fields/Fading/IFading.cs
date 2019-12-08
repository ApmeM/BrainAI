namespace BrainAI.Pathfinding.Fields.Fading
{
    public interface IFading
    {
        Point GetForce(Point vector, float chargeValue);
    }
}