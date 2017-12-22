using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame
{
    public abstract class GameSubState
    {
        protected GameState parentState;

        public GameSubState(GameState _parent)
        {
            parentState = _parent;
        }

        public abstract void PreRun(Game game, Player curPlayer);
        public abstract Action RequestAction(Game game, Player curPlayer);
        public abstract void HandleRequestAction(Action action, Game game, Player curPlayer);
        public abstract void SetNextSubstate(Game game, Player curPlayer);

        public virtual void Init() { }

        public virtual bool CheckCloneEquality(GameSubState substate)
        {
            bool equal = true;

            equal &= !object.ReferenceEquals(this, substate);
            equal &= parentState.StateType == substate.parentState.StateType;

            return equal;
        }
    }
}
