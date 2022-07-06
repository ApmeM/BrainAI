using System.Collections.Generic;

namespace BrainAI.Simulations
{
    public interface IPlayer<TState, TAction>
    {
        List<TAction> AvailableActions(TState state);
    }
}