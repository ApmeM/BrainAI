using System.Collections.Generic;

namespace BrainAI.Simulations
{
    public interface IPlayer<TState, TAction>
    {
        /// <summary>
        /// Available players actions in current game state. 
        /// It should be a subset of {@see IGame.AllAvailableActions} currently available for player.
        /// </summary>
        List<TAction> AvailableActions(TState state);
        
        /// <summary>
        /// Returns a score for the player in current state. The more the better.
        /// </summary>
        int Score(TState state);
    }
}