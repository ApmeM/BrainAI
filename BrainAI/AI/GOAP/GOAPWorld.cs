namespace BrainAI.AI.GOAP
{
    using System.Collections.Generic;

    public class GOAPWorld
    {
        private static readonly GOAPStorage Storage = new GOAPStorage();

        /* from: http://theory.stanford.edu/~amitp/GameProgramming/ImplementationNotes.html
        OPEN = priority queue containing START
        CLOSED = empty set
        while lowest rank in OPEN is not the GOAL:
          current = remove lowest rank item from OPEN
          add current to CLOSED
          for neighbors of current:
            cost = g(current) + movementcost(current, neighbor)
            if neighbor in OPEN and cost less than g(neighbor):
              remove neighbor from OPEN, because new path is better
            if neighbor in CLOSED and cost less than g(neighbor): **
              remove neighbor from CLOSED
            if neighbor not in OPEN and neighbor not in CLOSED:
              set g(neighbor) to cost
              add neighbor to OPEN
              set priority queue rank to g(neighbor) + h(neighbor)
              set neighbor's parent to current
        */

        /// <summary>
        /// Make a plan of actions that will reach desired world state
        /// </summary>
        /// <param name="ap">Ap.</param>
        /// <param name="start">Start.</param>
        /// <param name="goal">Goal.</param>
        /// <param name="selectedNodes">Storage.</param>
        public static Stack<GOAPAction> Plan( ActionPlanner ap, WorldState start, WorldState goal, List<GOAPNode> selectedNodes = null )
        {
            Storage.Clear();

            var currentNode = new GOAPNode();
            currentNode.WorldState = start;
            currentNode.ParentWorldState = start;
            currentNode.CostSoFar = 0; // g
            currentNode.HeuristicCost = CalculateHeuristic( start, goal ); // h
            currentNode.CostSoFarAndHeuristicCost = currentNode.CostSoFar + currentNode.HeuristicCost; // f
            currentNode.Depth = 1;

            Storage.AddToOpenList( currentNode );

            while( true )
            {
                // nothing left open so we failed to find a path
                if( !Storage.HasOpened() )
                {
                    Storage.Clear();
                    return null;
                }

                currentNode = Storage.RemoveCheapestOpenNode();

                Storage.AddToClosedList( currentNode );

                // all done. we reached our goal
                if( goal.Equals( currentNode.WorldState ) )
                {
                    var plan = ReconstructPlan( currentNode, selectedNodes );
                    Storage.Clear();
                    return plan;
                }

                var neighbors = ap.GetPossibleTransitions( currentNode.WorldState );
                for( var i = 0; i < neighbors.Count; i++ )
                {
                    var cur = neighbors[i];
                    var opened = Storage.FindOpened( cur );
                    var closed = Storage.FindClosed( cur );
                    var cost = currentNode.CostSoFar + cur.CostSoFar;

                    // if neighbor in OPEN and cost less than g(neighbor):
                    if( opened != null && cost < opened.CostSoFar )
                    {
                        // remove neighbor from OPEN, because new path is better
                        Storage.RemoveOpened( opened );
                        opened = null;
                    }

                    // if neighbor in CLOSED and cost less than g(neighbor):
                    if( closed != null && cost < closed.CostSoFar )
                    {
                        // remove neighbor from CLOSED
                        Storage.RemoveClosed( closed );
                    }

                    // if neighbor not in OPEN and neighbor not in CLOSED:
                    if( opened == null && closed == null )
                    {
                        var nb = new GOAPNode
                        {
                            WorldState = cur.WorldState,
                            CostSoFar = cost,
                            HeuristicCost = CalculateHeuristic(cur.WorldState, goal),
                            Action = cur.Action,
                            ParentWorldState = currentNode.WorldState,
                            Parent = currentNode,
                            Depth = currentNode.Depth + 1
                        };
                        nb.CostSoFarAndHeuristicCost = nb.CostSoFar + nb.HeuristicCost;
                        Storage.AddToOpenList( nb );
                    }
                }
            }
        }


        /// <summary>
        /// internal function to reconstruct the plan by tracing from last node to initial node
        /// </summary>
        /// <returns>The plan.</returns>
        private static Stack<GOAPAction> ReconstructPlan( GOAPNode goalNode, List<GOAPNode> selectedNodes )
        {
            var totalActionsInPlan = goalNode.Depth - 1;
            var plan = new Stack<GOAPAction>( totalActionsInPlan );

            var curnode = goalNode;
            for( var i = 0; i <= totalActionsInPlan - 1; i++ )
            {
                // optionally add the node to the List if we have been passed one
                selectedNodes?.Add( curnode.Clone() );
                plan.Push( curnode.Action );
                curnode = curnode.Parent;
            }

            // our nodes went from the goal back to the start so reverse them
            selectedNodes?.Reverse();

            return plan;
        }


        /// <summary>
        /// This is our heuristic: estimate for remaining distance is the nr of mismatched atoms that matter.
        /// </summary>
        /// <returns>The heuristic.</returns>
        private static int CalculateHeuristic( WorldState @from, WorldState to )
        {
            long care = ( to.DontCare ^ -1L );
            long diff = ( @from.Values & care ) ^ ( to.Values & care );
            int dist = 0;

            for( var i = 0; i < ActionPlanner.MaxConditions; ++i )
                if( ( diff & ( 1L << i ) ) != 0 )
                    dist++;
            return dist;
        }

    }
}

