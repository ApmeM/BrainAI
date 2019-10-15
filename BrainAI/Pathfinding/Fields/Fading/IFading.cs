namespace BrainAI.Pathfinding.Fields.Fading
{
    public interface IFading
    {
        Point GetForce(Point chargePoint, float chargeValue, Point atPosition);
    }
}