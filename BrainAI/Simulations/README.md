Simulations
==========
BrainAI provides 2 few simulation type decision making algorithms:

- Minimax
- Smitsimax

Both algoritms require to implement 2 interfaces - IGame (game rules) and IPlayer (player actions).

IGame:

- ApplyAction - game simulation step. It takes original game state and all player moves (for some games it is possible to move all players at once) and return new state based on players moves.
- Score - calculates state score for the seected player.
- IsGameOver - check that for specified state game is actually over to stop further calculations.
- AllAvailableActions - list all available actions of the game.

IPlayer:

- AvailableActions for specified state list all currently available actions. It should be a subset of IGame.AllAvailableActions.

## Minimax

Minimax is a decision making algorithm used in artificial intelligence for minimizing the possible loss for a worst case (maximum loss) scenario. Current iplementation works for the game where players moves one by one.

In addition to basic full search algorithm it contains an extension as alpha-beta pruning - a search algorithm that tries to decrease the number of nodes that are evaluated by the minimax algorithm in its search tree. It stops evaluating a move when at least one possibility has been found that proves the move to be worse than a previously examined move. Such moves need not be evaluated further. When applied to a standard minimax tree, it returns the same move as minimax would, but prunes away branches that cannot possibly influence the final decision.

## Smitsimax

Smitsimax is another decision making algorithm that build a tree based on all players moves. Each player estimate the turn and the best move will be selected.

## Example.

The fully functional test example can be found in [BaseSimulationTest.cs](../../BrainAI.Tests/BaseSimulationTest.cs) file.

