using System.Collections.Generic;

namespace BrainAI.Simulations
{
    public interface IPlayer<TState, TAction>
    {
        List<TAction> AvailableActions(TState state);
        TState ApplyAction(TState state, TAction selectedAction);
        int Score(TState state);
    }
}