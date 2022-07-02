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
            private readonly FieldState playerValue;
            private readonly FieldState opponentValue;

            public int ScoreRequested = 0;

            public TicTacToePlayer(FieldState playerValue, FieldState opponentValue)
            {
                this.playerValue = playerValue;
                this.opponentValue = opponentValue;
            }

            public List<int> AvailableActions(GameState state)
            {
                if (Score(state) != 0) {
                    return null;
                }

                var actions = state.Field.Cast<FieldState>()
                    .Select((FieldState a, int b) => new Tuple<FieldState, int>(a, b))
                    .Where(a => a.Item1 == FieldState._)
                    .Select(a => a.Item2)
                    .ToList();

                if (actions.Count == 0){
                    return null;
                }

                return actions;
            }

            public GameState ApplyAction(GameState state, int selectedAction)
            {
                var result = new GameState();
                result.Field = state.Field.ToArray();
                result.Field[selectedAction] = this.playerValue;
                return result;
            }

            public int Score(GameState state)
            {
                ScoreRequested ++;
                if (CheckWinner(state, this.playerValue))
                {
                    return 100;
                }

                if (CheckWinner(state, this.opponentValue))
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

            var gamePlayer = new TicTacToePlayer(FieldState.X, FieldState.O);
            var target = new MinimaxSimulation<GameState, int>();
            var result = target.Minimax(1, gameState, gamePlayer, gamePlayer);

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

            var gamePlayerX = new TicTacToePlayer(FieldState.X, FieldState.O);
            var gamePlayerO = new TicTacToePlayer(FieldState.O, FieldState.X);
            var target = new MinimaxSimulation<GameState, int>();
            var result = target.Minimax(20, gameState, gamePlayerO, gamePlayerX);

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

            var gamePlayerX = new TicTacToePlayer(FieldState.X, FieldState.O);
            var gamePlayerO = new TicTacToePlayer(FieldState.O, FieldState.X);
            var target = new MinimaxSimulation<GameState, int>();
            var result = target.Minimax(20, gameState, gamePlayerX, gamePlayerO);

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

            var gamePlayerX = new TicTacToePlayer(FieldState.X, FieldState.O);
            var gamePlayerO = new TicTacToePlayer(FieldState.O, FieldState.X);
            var target = new MinimaxSimulation<GameState, int>();
            var result = target.Minimax(20, gameState, gamePlayerX, gamePlayerO);

            Assert.AreEqual(140, gamePlayerO.ScoreRequested + gamePlayerX.ScoreRequested);
        }
    }
}
