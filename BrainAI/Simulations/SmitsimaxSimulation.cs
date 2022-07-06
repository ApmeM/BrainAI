using System;
using System.Collections.Generic;
using System.Linq;

namespace BrainAI.Simulations
{
    public class SmitsimaxSimulation<TState, TAction>
    {
        private readonly float exploration_param;

        public SmitsimaxSimulation(float exploration_param)
        {
            this.exploration_param = exploration_param;
        }

        private class Node
        {
            internal int Visits;
            internal Node Parent;
            internal float Score;
            internal TAction Action;
            internal List<Node> Children;
            internal float ChildMinScore = float.MaxValue;
            internal float ChildMaxScore = float.MinValue;
        }

        public TAction Minimax(
            int depth,
            int triesCount,
            TState initialState,
            IGame<TState, TAction> game,
            params IPlayer<TState, TAction>[] players)
        {
            var rootNodes = players
                .Select(a => new Node { Visits = 1 })
                .ToArray();

            var availableActions = game.AllAvailableActions();
                
            var actions = new Tuple<IPlayer<TState, TAction>, TAction>[players.Length];
            var currentNodes = new Node[players.Length];

            for (var t = 0; t < triesCount; t++)
            {
                var currentState = initialState;
                for (int playerId = 0; playerId < players.Length; playerId++)
                {
                    currentNodes[playerId] = rootNodes[playerId];
                }

                for (var curDepth = 0; curDepth < depth; curDepth++)
                {
                    for (int playerId = 0; playerId < players.Length; ++playerId)
                    {
                        Node node = currentNodes[playerId];

                        if (node.Children == null)
                        {
                            MakeChildren(node, availableActions);
                        }

                        var child = Select(node, players[playerId].AvailableActions(currentState));

                        ++child.Visits;
                        currentNodes[playerId] = child;
                        actions[playerId] = new Tuple<IPlayer<TState, TAction>, TAction>(players[playerId], child.Action);
                    }

                    currentState = game.ApplyAction(currentState, actions);

                    if (game.IsGameOver(currentState))
                    {
                        break;
                    }
                }

                for (int playerId = 0; playerId < players.Length; ++playerId)
                {
                    int score = game.Score(currentState, players[playerId]);

                    // Backpropagate
                    while (currentNodes[playerId].Parent != null)
                    {
                        currentNodes[playerId].Score += score;
                        currentNodes[playerId].Parent.ChildMinScore = Math.Min(currentNodes[playerId].Parent.ChildMinScore, score);
                        currentNodes[playerId].Parent.ChildMaxScore = Math.Max(currentNodes[playerId].Parent.ChildMaxScore, score);
                        currentNodes[playerId] = currentNodes[playerId].Parent;
                    }
                }
            }

            Node best = null;
            var maxScore = float.MinValue;

            foreach (var nod in rootNodes[0].Children)
            {
                if (nod.Score > maxScore)
                {
                    maxScore = nod.Score;
                    best = nod;
                }
            }
            return best.Action;
        }

        private void MakeChildren(Node node, List<TAction> availableActions)
        {
            node.Children = availableActions
                .Select(a => new Node { Parent = node, Action = a })
                .ToList();
        }

        private Node Select(Node parent, List<TAction> availableActions)
        {
            var children = parent.Children.Where(a=>availableActions.Contains(a.Action)).ToList();
            foreach (var child in children)
            {
                if (child.Visits == 0)
                {
                    return child;
                }
            }

            if (parent.ChildMinScore == parent.ChildMaxScore)
            {
                // Possible scenarios:
                // - Single child. Just take it.
                // - All childs have same score. Just take first
                return children[0];
            }

            Node best = null;
            var maxucb = float.MinValue;

            foreach (var child in children)
            {
                var normalizedScore = (child.Score - parent.ChildMinScore) / (parent.ChildMaxScore - parent.ChildMinScore);
                var ucb = normalizedScore / child.Visits + (float)(this.exploration_param * Math.Sqrt(Math.Log(parent.Visits)) / Math.Sqrt(child.Visits));

                if (ucb > maxucb)
                {
                    maxucb = ucb;
                    best = child;
                }
            }

            return best;
        }
    }
}