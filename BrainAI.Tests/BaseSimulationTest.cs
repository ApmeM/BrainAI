using BrainAI.Simulations;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BrainAI.Tests
{
    [TestFixture]
    public class BaseSimulationTest
    {
        enum FieldState
        {
            X, O, _
        }

        struct GameState
        {
            public FieldState[] Field;
        }

        class TicTacToePlayer : IPlayer<GameState, int>
        {
            public readonly FieldState playerValue;

            public TicTacToePlayer(FieldState playerValue)
            {
                this.playerValue = playerValue;
            }

            public List<int> AvailableActions(GameState state)
            {
                return state.Field.Cast<FieldState>()
                    .Select((FieldState a, int b) => new Tuple<FieldState, int>(a, b))
                    .Where(a => a.Item1 == FieldState._)
                    .Select(a => a.Item2)
                    .ToList();
            }
        }

        class TicTacToeGame : IGame<GameState, int>
        {
            public int ScoreRequested = 0;

            public GameState ApplyAction(GameState state, params Tuple<IPlayer<GameState, int>, int>[] selectedActions)
            {
                var xCount = state.Field.Cast<FieldState>()
                    .Select((FieldState a, int b) => new Tuple<FieldState, int>(a, b))
                    .Where(a => a.Item1 == FieldState.X)
                    .Select(a => a.Item2)
                    .Count();
                var oCount = state.Field.Cast<FieldState>()
                    .Select((FieldState a, int b) => new Tuple<FieldState, int>(a, b))
                    .Where(a => a.Item1 == FieldState.O)
                    .Select(a => a.Item2)
                    .Count();
                var _Count = state.Field.Cast<FieldState>()
                    .Select((FieldState a, int b) => new Tuple<FieldState, int>(a, b))
                    .Where(a => a.Item1 == FieldState._)
                    .Select(a => a.Item2)
                    .Count();

                var playerAction = selectedActions.Length == 1 ? selectedActions[0] :
                                                xCount == oCount ? selectedActions[0] : 
                                                selectedActions[1];

                var selectedAction = playerAction.Item2;
                var player = (TicTacToePlayer)playerAction.Item1;
                var result = new GameState();
                result.Field = state.Field.ToArray();
                if (result.Field[selectedAction] == FieldState._)
                {
                    result.Field[selectedAction] = player.playerValue;
                }

                return result;
            }

            public List<int> AllAvailableActions()
            {
                return new List<int>{
                    0,1,2,3,4,5,6,7,8
                };
            }

            public bool IsGameOver(GameState state)
            {
                return state.Field.All(a => a != FieldState._) || CheckWinner(state, FieldState.X) || CheckWinner(state, FieldState.O);
            }

            public int Score(GameState state, IPlayer<GameState, int> player)
            {
                ScoreRequested++;

                if (CheckWinner(state, ((TicTacToePlayer)player).playerValue))
                {
                    return 100;
                }

                var opponentValue = ((TicTacToePlayer)player).playerValue == FieldState.X ? FieldState.O : FieldState.X;

                if (CheckWinner(state, opponentValue))
                {
                    return -100;
                }

                return 0;
            }

            private bool CheckWinner(GameState state, FieldState fieldState)
            {
                return
                    state.Field[0] == fieldState && state.Field[1] == fieldState && state.Field[2] == fieldState ||
                    state.Field[0] == fieldState && state.Field[3] == fieldState && state.Field[6] == fieldState ||
                    state.Field[0] == fieldState && state.Field[4] == fieldState && state.Field[8] == fieldState ||
                    state.Field[1] == fieldState && state.Field[4] == fieldState && state.Field[7] == fieldState ||
                    state.Field[2] == fieldState && state.Field[5] == fieldState && state.Field[8] == fieldState ||
                    state.Field[3] == fieldState && state.Field[4] == fieldState && state.Field[5] == fieldState ||
                    state.Field[4] == fieldState && state.Field[2] == fieldState && state.Field[6] == fieldState ||
                    state.Field[6] == fieldState && state.Field[7] == fieldState && state.Field[8] == fieldState;
            }
        }

        [Test]
        public void FindBestAction_SinglePlayerOneStep_BestActionSelected()
        {
            var gameState = new GameState
            {
                Field = new FieldState[]
                {
                    FieldState.X, FieldState.X, FieldState._,
                    FieldState.O, FieldState.O, FieldState.X,
                    FieldState._, FieldState._, FieldState.O,
                }
            };

            var game = new TicTacToeGame();
            var gamePlayer = new TicTacToePlayer(FieldState.X);
            var target = new MinimaxSimulation<GameState, int>();
            var result = target.Minimax(1, gameState, game, gamePlayer, gamePlayer);

            Assert.AreEqual(2, result);
        }

        [Test]
        public void FindBestAction_MultiPlayerSingleBestResult_BestActionSelected()
        {
            var gameState = new GameState
            {
                Field = new FieldState[]
                {
                    FieldState.X, FieldState._, FieldState._,
                    FieldState._, FieldState._, FieldState._,
                    FieldState._, FieldState._, FieldState._,
                }
            };

            var game = new TicTacToeGame();
            var gamePlayerX = new TicTacToePlayer(FieldState.X);
            var gamePlayerO = new TicTacToePlayer(FieldState.O);
            var target = new MinimaxSimulation<GameState, int>();
            var result = target.Minimax(20, gameState, game, gamePlayerO, gamePlayerX);

            Assert.AreEqual(4, result);
        }

        [Test]
        public void FindBestAction_MultiPlayerSingleDrawResult_BestActionSelected()
        {
            var gameState = new GameState
            {
                Field = new FieldState[]
                {
                    FieldState.X, FieldState.O, FieldState.X,
                    FieldState._, FieldState.O, FieldState._,
                    FieldState._, FieldState._, FieldState._,
                }
            };

            var game = new TicTacToeGame();
            var gamePlayerX = new TicTacToePlayer(FieldState.X);
            var gamePlayerO = new TicTacToePlayer(FieldState.O);
            var target = new MinimaxSimulation<GameState, int>();
            var result = target.Minimax(20, gameState, game, gamePlayerX, gamePlayerO);

            Assert.AreEqual(7, result);
        }

        [Test]
        public void FindBestAction_ScoreRequestedWithoutAlphabeta()
        {
            var gameState = new GameState
            {
                Field = new FieldState[]
                {
                    FieldState.X, FieldState.O, FieldState.X,
                    FieldState._, FieldState.O, FieldState._,
                    FieldState._, FieldState._, FieldState._,
                }
            };

            var game = new TicTacToeGame();
            var gamePlayerX = new TicTacToePlayer(FieldState.X);
            var gamePlayerO = new TicTacToePlayer(FieldState.O);
            var target = new MinimaxSimulation<GameState, int>();

            game.ScoreRequested = 0;
            var result = target.Minimax(20, gameState, game, gamePlayerX, gamePlayerO);
            Assert.AreEqual(39, game.ScoreRequested);
            game.ScoreRequested = 0;
            var resultFullSearch = target.MinimaxFullSearch(20, gameState, game, gamePlayerX, gamePlayerO);
            Assert.AreEqual(92, game.ScoreRequested);
        }

        [Test]
        public void Aaa()
        {
            var gameState = new GameState
            {
                Field = new FieldState[]
                {
                    FieldState.X, FieldState.O, FieldState.X,
                    FieldState._, FieldState.O, FieldState._,
                    FieldState._, FieldState._, FieldState._,
                }
            };

            var game = new TicTacToeGame();
            var gamePlayerX = new TicTacToePlayer(FieldState.X);
            var gamePlayerO = new TicTacToePlayer(FieldState.O);
            var target = new SmitsimaxSimulation<GameState, int>(0.5f);

            var result = target.Minimax(10, 500, gameState, game, gamePlayerX, gamePlayerO);
            Assert.AreEqual(7, result);
        }
    }
}
