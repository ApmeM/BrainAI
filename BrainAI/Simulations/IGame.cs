using System;
using System.Collections.Generic;

namespace BrainAI.Simulations
{
    public interface IGame<TState, TAction>
    {
        TState ApplyAction(TState state, params Tuple<IPlayer<TState, TAction>, TAction>[] selectedActions);
        int Score(TState state, IPlayer<TState, TAction> player);
        bool IsGameOver(TState state);
        List<TAction> AllAvailableActions();
    }
}