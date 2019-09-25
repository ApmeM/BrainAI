namespace BrainAI.AI.GOAP
{
    using System;

    public class GOAPNode : IComparable<GOAPNode>, IEquatable<GOAPNode>
    {
        /// <summary>
        /// The state of the world at this node.
        /// </summary>
        public WorldState WorldState;

        /// <summary>
        /// The cost so far.
        /// </summary>
        public int CostSoFar;

        /// <summary>
        /// The heuristic for remaining cost (don't overestimate!)
        /// </summary>
        public int HeuristicCost;

        /// <summary>
        /// costSoFar + heuristicCost (g+h) combined.
        /// </summary>
        public int CostSoFarAndHeuristicCost;

        /// <summary>
        /// the Action associated with this node
        /// </summary>
        public GOAPAction Action;

        // Where did we come from?
        public GOAPNode Parent;
        public WorldState ParentWorldState;
        public int Depth;

        #region IEquatable and IComparable

        public bool Equals( GOAPNode other )
        {
            var care = this.WorldState.DontCare ^ -1L;
            return ( this.WorldState.Values & care ) == ( other.WorldState.Values & care );
        }


        public int CompareTo( GOAPNode other )
        {
            return this.CostSoFarAndHeuristicCost.CompareTo( other.CostSoFarAndHeuristicCost );
        }

        #endregion

        public void Reset()
        {
            this.Action = null;
            this.Parent = null;
        }

        public GOAPNode Clone()
        {
            return (GOAPNode)this.MemberwiseClone();
        }

        public override string ToString()
        {
            return $"[cost: {this.CostSoFar} | heuristic: {this.HeuristicCost}]: {this.Action}";
        }
    }
}