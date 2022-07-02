using System;

namespace BrainAI.Simulations
{
    public class MinimaxSimulation<TState, TAction>
    {
        public TAction Minimax(
            int depth,
            TState initialState,
            IPlayer<TState, TAction> maximizingPlayer,
            IPlayer<TState, TAction> minimizingPlayer)
        {
            var currentPlayer = maximizingPlayer;
            return Minimax(depth, initialState, int.MinValue, int.MaxValue, maximizingPlayer, minimizingPlayer, currentPlayer).Item1;
        }

        private ValueTuple<TAction, int> Minimax(
            int depth,
            TState currentState,
            int alpha,
            int beta,
            IPlayer<TState, TAction> maximizingPlayer,
            IPlayer<TState, TAction> minimizingPlayer,
            IPlayer<TState, TAction> currentPlayer)
        {
            if (depth == 0)
            {
                return new ValueTuple<TAction, int>(default(TAction), maximizingPlayer.Score(currentState));
            }

            var availableActions = currentPlayer.AvailableActions(currentState);
            if (availableActions == null)
            {
                return new ValueTuple<TAction, int>(default(TAction), maximizingPlayer.Score(currentState));
            }

            if (currentPlayer == maximizingPlayer)
            {
                var value = int.MinValue;
                var bestAction = default(TAction);
                foreach (var action in availableActions)
                {
                    var newState = currentPlayer.ApplyAction(currentState, action);
                    var newValue = Minimax(depth - 1, newState, alpha, beta, maximizingPlayer, minimizingPlayer, minimizingPlayer).Item2;
                    if (newValue > value)
                    {
                        value = newValue;
                        bestAction = action;
                    }

                    if (value >= beta)
                    {
                        break;
                    }
                    alpha = Math.Max(alpha, value);
                }
                return new ValueTuple<TAction, int>(bestAction, value);
            }
            else
            {
                var value = int.MaxValue;
                var bestAction = default(TAction);
                foreach (var action in availableActions)
                {
                    var newState = currentPlayer.ApplyAction(currentState, action);
                    var newValue = Minimax(depth - 1, newState, alpha, beta, maximizingPlayer, minimizingPlayer, maximizingPlayer).Item2;
                    if (newValue < value)
                    {
                        value = newValue;
                        bestAction = action;
                    }

                    if (value <= alpha)
                    {
                        break;
                    }
                    beta = Math.Min(beta, value);
                }
                return new ValueTuple<TAction, int>(bestAction, value);
            }
        }
    }
}