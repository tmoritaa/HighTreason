using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame
{
    public abstract class GameState
    {
        protected Game game;

        public GameState(Game _game)
        {
            game = _game;
        }

        public abstract void GotoNextState();
        public abstract void StartState();
    }
}
