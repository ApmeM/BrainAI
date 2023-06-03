namespace BrainAI.Pathfinding
{
    using System.Collections.Generic;

    public interface IPathfinder<T>
    {
        /// Search path from start to goal and return as a list of steps.
        ///
        /// Result list contains both start and goal elements. List[0] == start, List[end] == goal
        /// If path was not found - null will be returned.
        ///
        /// Method is NOT thread safe. In case of parallel usage the result might be unpredictable.
        /// Result list is the same instance for all method calls. 
        /// Be sure you copy the result before next search or the result will be lost.
        ///
        /// WARNING: if there is no path to the goal on infinity map this method fails to infinity loop.
        List<T> Search(T start, T goal);
    }
}