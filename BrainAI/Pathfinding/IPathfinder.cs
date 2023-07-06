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

        /// Contains data about all visited nodes during last search process.
        /// All 'Search' methods cleanup this field at the very beginning.
        /// All 'ContinueSearch' methods reuse this field to continue search process.
        ///
        /// Content of this dictionary can be used to build paths using PathConstructor method.
        /// Start point should be the point passed to 'Search' method.
        /// End point can be any point that exists in Visitednodes keys.
        Dictionary<T, T> VisitedNodes { get; }

        /// Search path from start to CLOSEST goal from set and return as a list of steps.
        ///
        /// Result list contains both start and goal elements. List[0] == start, List[end] == CLOSEST goal
        /// If path was not found OR there is no goals - null will be returned.
        ///
        /// Method is NOT thread safe. In case of parallel usage the result might be unpredictable.
        /// Result list is the same instance for all method calls. 
        /// Be sure you copy the result before next search or the result will be lost.
        ///
        /// WARNING: if there is no path to the goals on infinity map this method fails to infinity loop.
        List<T> Search(T start, HashSet<T> goals);

        /// Search path from start to goal and return as a list of steps WITHIN a specified distance.
        ///
        /// Result list contains both start and goal elements. List[0] == start, List[end] == CLOSEST goal
        /// If path was not found OR there is no goals - null will be returned.
        ///
        /// Method is NOT thread safe. In case of parallel usage the result might be unpredictable.
        /// Result list is the same instance for all method calls. 
        /// Be sure you copy the result before next search or the result will be lost.
        List<T> Search(T start, T goal, int maxPathWeight);

        /// Search path from start to CLOSEST goal from set and return as a list of steps WITHIN a specified distance.
        ///
        /// Result list contains both start and goal elements. List[0] == start, List[end] == CLOSEST goal
        /// If path was not found OR there is no goals - null will be returned.
        ///
        /// Method is NOT thread safe. In case of parallel usage the result might be unpredictable.
        /// Result list is the same instance for all method calls. 
        /// Be sure you copy the result before next search or the result will be lost.
        List<T> Search(T start, HashSet<T> goals, int maxPathWeight);

        /// Continues search process from start point passed in original 'Search' until next goal in goals set reached.
        ///
        /// Result list contains both start and goal elements. List[0] == start, List[end] == CLOSEST goal
        /// If path was not found OR there is no goals - null will be returned.
        ///
        /// WARNING: if there is no path to the goal on infinity map this method fails to infinity loop.
        List<T> ContinueSearch();

        /// Continues search process from start point passed in original 'Search' within ADDITIONAL maxPathWeight distance.
        ///
        /// Result list contains both start and goal elements. List[0] == start, List[end] == CLOSEST goal
        /// If path was not found OR there is no goals - null will be returned.
        List<T> ContinueSearch(int maxPathWeight);

    }
}