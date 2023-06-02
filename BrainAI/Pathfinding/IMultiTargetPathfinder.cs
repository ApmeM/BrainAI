namespace BrainAI.Pathfinding
{
    using System.Collections.Generic;

    public interface IMultiTargetPathfinder<T> : IPathfinder<T>
    {
        /// Search path from start to CLOSEST goal from set and return as a list of steps.
        ///
        /// Result list contains both start and goal elements. List[0] == start, List[end] == CLOSEST goal
        /// If path was not found - null will be returned.
        ///
        /// Method is NOT thread safe. In case of parallel usage the result might be unpredictable.
        /// Result list is the same instance for all method calls. 
        /// Be sure you copy the result before next search or the result will be lost.
        List<T> Search(T start, HashSet<T> goals);
        
        /// Find all achievable paths from start point within maxPathWeight distance.
        ///
        /// Dictionary element is a pair of from and to steps for single graph element.
        /// Using this dictionary PathConstructor can build path from start to any covered point.
        ///
        /// Method is NOT thread safe. In case of parallel usage the result might be unpredictable.
        /// Result dictionary is the same instance for all method calls. 
        /// Be sure you copy the result before next search or the result will be lost.
        Dictionary<T, T> Search(T start, int maxPathWeight);

        /// Find all achievable paths from start point within maxPathWeight distance OR until CLOSEST goal point reached.
        ///
        /// Dictionary element is a pair of from and to steps for single graph element.
        /// Using this dictionary PathConstructor can build path from start to any covered point.
        ///
        /// Method is NOT thread safe. In case of parallel usage the result might be unpredictable.
        /// Result dictionary is the same instance for all method calls. 
        /// Be sure you copy the result before next search or the result will be lost.
        Dictionary<T, T> Search(T start, HashSet<T> goals, int maxPathWeight);
    }
}