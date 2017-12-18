using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame
{
    public abstract class GameSubState
    {
        protected GameState parentState;
        protected object actionResponse;

        public GameSubState(GameState _parent)
        {
            parentState = _parent;
        }

        public abstract void PreRun(Game game, Player curPlayer);
        public abstract Action RequestAction(Game game, Player curPlayer);
        public abstract void HandleRequestAction(Action action);
        public abstract void RunRest(Game game, Player curPlayer);
        public abstract void PrepareNextSubstate();

        public virtual void Init() { }
    }
}
