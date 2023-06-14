namespace BrainAI.Pathfinding
{
    using System;

    /// <summary>
    /// Basic implementation Graph with edges for Point type. 
    /// In addition to basic EdgesGraph it can have a heuristic that helps in pathfinding
    /// </summary>
    public class EdgesPointGraph : EdgesGraph<Point>, IAstarGraph<Point>
    {
        public int Heuristic(Point node, Point goal)
        {
            return (Math.Abs(node.X - goal.X) + Math.Abs(node.Y - goal.Y)) * this.DefaultWeight;
        }
    }
}

