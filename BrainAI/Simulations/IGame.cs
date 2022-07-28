using System;
using System.Collections.Generic;

namespace BrainAI.Simulations
{
    public interface IGame<TState, TAction>
    {
        /// <summary>
        /// Applies players selected actions to the game state and returns new state as a result.
        /// </summary>
        TState ApplyAction(TState state, params Tuple<IPlayer<TState, TAction>, TAction>[] selectedActions);

        /// <summary>
        /// Checks if the state represents finished game and further calculations should not be done
        /// </summary>
        bool IsGameOver(TState state);

        /// <summary>
        /// List all availables moves for the game regardless of state. 
        /// For exampe for TicTacToe game there is 9 possible moves and all of them should be listed here.
        /// </summary>
        List<TAction> AllAvailableActions();
    }
}