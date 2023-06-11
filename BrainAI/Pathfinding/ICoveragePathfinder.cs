namespace BrainAI.Pathfinding
{
    public interface ICoveragePathfinder<T> : IPathfinder<T>
    {
        /// Find all achievable paths from start point within maxPathWeight distance.
        ///
        /// The result dictionary of all visited paths can be found in VisitedPath property.
        /// VisitedPath Dictionary element is a pair of from and to steps for single graph element.
        /// Using this dictionary PathConstructor can build path from start to any covered point.
        ///
        /// Method is NOT thread safe. In case of parallel usage the result might be unpredictable.
        /// VisitedPath dictionary is the same instance for all method calls. 
        /// Be sure you copy the result before next search or the result will be lost.
        void Search(T start, int maxPathWeight);
    }
}