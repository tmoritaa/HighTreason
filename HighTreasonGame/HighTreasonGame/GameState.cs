using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighTreasonGame
{
    public abstract class GameState
    {
        protected int gameId;

        public GameState(int _gameId)
        {
            gameId = _gameId;
        }

        public abstract void StartState();
        public abstract void GotoNextState();
    }
}
