using System;

namespace BrainAI.Simulations
{
    public class MinimaxSimulation<TState, TAction>
    {
        public TAction Minimax(
            int depth,
            TState initialState,
            IGame<TState, TAction> game,
            IPlayer<TState, TAction> maximizingPlayer,
            IPlayer<TState, TAction> minimizingPlayer)
        {
            var currentPlayer = maximizingPlayer;
            return Minimax(depth, initialState, game, int.MinValue, int.MaxValue, true, maximizingPlayer, minimizingPlayer, currentPlayer).Item1;
        }

        public TAction MinimaxFullSearch(
            int depth,
            TState initialState,
            IGame<TState, TAction> game,
            IPlayer<TState, TAction> maximizingPlayer,
            IPlayer<TState, TAction> minimizingPlayer)
        {
            var currentPlayer = maximizingPlayer;
            return Minimax(depth, initialState, game, int.MinValue, int.MaxValue, false, maximizingPlayer, minimizingPlayer, currentPlayer).Item1;
        }

        private ValueTuple<TAction, int> Minimax(
            int depth,
            TState currentState,
            IGame<TState, TAction> game,
            int alpha,
            int beta,
            bool useAlphaBeta,
            IPlayer<TState, TAction> maximizingPlayer,
            IPlayer<TState, TAction> minimizingPlayer,
            IPlayer<TState, TAction> currentPlayer)
        {
            if (depth == 0 || game.IsGameOver(currentState))
            {
                return new ValueTuple<TAction, int>(default(TAction), game.Score(currentState, maximizingPlayer));
            }

            var availableActions = currentPlayer.AvailableActions(currentState);

            if (currentPlayer == maximizingPlayer)
            {
                var value = int.MinValue;
                var bestAction = default(TAction);
                foreach (var action in availableActions)
                {
                    var newState = game.ApplyAction(currentState, new Tuple<IPlayer<TState, TAction>, TAction>(currentPlayer, action));
                    var newValue = Minimax(depth - 1, newState, game, alpha, beta, useAlphaBeta, maximizingPlayer, minimizingPlayer, minimizingPlayer).Item2;
                    if (newValue > value)
                    {
                        value = newValue;
                        bestAction = action;
                    }

                    if (value >= beta && useAlphaBeta)
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
                    var newState = game.ApplyAction(currentState, new Tuple<IPlayer<TState, TAction>, TAction>(currentPlayer, action));
                    var newValue = Minimax(depth - 1, newState, game, alpha, beta, useAlphaBeta, maximizingPlayer, minimizingPlayer, maximizingPlayer).Item2;
                    if (newValue < value)
                    {
                        value = newValue;
                        bestAction = action;
                    }

                    if (value <= alpha && useAlphaBeta)
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