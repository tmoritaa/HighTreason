using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighTreasonGame
{
    public abstract class GameState
    {
        public enum StateType
        {
            JurySelection,
            TrialInChief,
            Summation,
            Deliberation,
        }

        protected int gameId;

        public GameState(int _gameId)
        {
            gameId = _gameId;
        }

        public abstract void StartState();
        public abstract void GotoNextState();
    }
}
