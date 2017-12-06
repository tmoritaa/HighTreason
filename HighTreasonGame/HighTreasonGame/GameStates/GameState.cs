using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame
{
    public abstract class GameState
    {
        public enum GameStateType
        {
            JurySelection,
            JuryDismissal,
            TrialInChief,
            Summation,
            Deliberation,
        }

        protected Game game;

        public GameStateType StateType
        {
            get; protected set;
        }


        public GameState(GameStateType _stateType, Game _game)
        {
            game = _game;
            StateType = _stateType;
        }

        public abstract void GotoNextState();
        public abstract void StartState();
    }
}
