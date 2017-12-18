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

        public GameStateType StateType
        {
            get; protected set;
        }

        protected Game game;
        protected Player curPlayer;
        protected Dictionary<Type, GameSubState> substates = new Dictionary<Type, GameSubState>();

        protected bool stateEnded = false;

        public GameSubState CurSubstate
        {
            get; protected set;
        }

        public GameState(GameStateType _stateType, Game _game)
        {
            game = _game;
            StateType = _stateType;
            initSubStates(this);
        }

        public abstract void GotoNextState();

        public virtual void InitState()
        {
            foreach (var substate in substates.Values)
            {
                substate.Init();
            }
        }

        public Action Start()
        {
            CurSubstate.PreRun(game, curPlayer);
            return CurSubstate.RequestAction(game, curPlayer);
        }

        public Action Continue(Action action)
        {
            Action response = action;
            while (!stateEnded)
            {
                CurSubstate.HandleRequestAction(response);
                CurSubstate.RunRest(game, curPlayer);
                CurSubstate.PrepareNextSubstate();

                if (stateEnded)
                {
                    break;
                }

                CurSubstate.PreRun(game, curPlayer);
                Action a = CurSubstate.RequestAction(game, curPlayer);
                if (a != null)
                {
                    return a;
                }
                else
                {
                    response = null;
                }
            }

            GotoNextState();
            if (game.GameEnd)
            {
                return null;
            }
            else
            {
                
                return game.CurState.Start();
            }
        }

        public void SetNextSubstate(Type type)
        {
            CurSubstate = substates[type];
        }

        public void SignifyStateEnd()
        {
            stateEnded = true;
        }

        public void PassToNextPlayer()
        {
            curPlayer = game.GetOtherPlayer(curPlayer);
        }

        protected abstract void initSubStates(GameState parent);
    }
}
