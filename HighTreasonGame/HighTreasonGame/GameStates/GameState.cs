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

        // Copy constructor
        public GameState(GameState state, Game _game)
        {
            game = _game;
            StateType = state.StateType;
            stateEnded = state.stateEnded;

            if (state.curPlayer != null)
            {
                curPlayer = _game.GetPlayerOfSide(state.curPlayer.Side);
            }

            copySubstates(state, _game);

            if (state.CurSubstate != null)
            {
                CurSubstate = substates[state.CurSubstate.GetType()];
            }
        }

        public virtual bool CheckCloneEquality(GameState state)
        {
            bool equal = true;

            equal &= !object.ReferenceEquals(game, state.game);
            equal &= !object.ReferenceEquals(this, state);

            if (!equal)
            {
                Console.WriteLine("References were not equal");
                return equal;
            }

            equal &= StateType == state.StateType;
            equal &= stateEnded == state.stateEnded;

            if (!equal)
            {
                Console.WriteLine("Vars were not equal");
                return equal;
            }

            if (curPlayer != null)
            {
                equal &= curPlayer.Side == state.curPlayer.Side;
            }
            else
            {
                equal &= curPlayer == state.curPlayer;
            }

            if (!equal)
            {
                Console.WriteLine("curPlayer was not equal");
                return equal;
            }

            if (CurSubstate != null)
            {
                equal &= CurSubstate.GetType() == state.CurSubstate.GetType();
            }
            else
            {
                equal &= CurSubstate == state.CurSubstate;
            }

            if (!equal)
            {
                Console.WriteLine("CurSubstate was not equal");
                return equal;
            }

            foreach (var kv in substates)
            {
                kv.Value.CheckCloneEquality(state.substates[kv.Key]);
                if (!equal)
                {
                    Console.WriteLine("Substate " + kv.Value.GetType() + " were not equal");
                    return equal;
                }
            }

            return equal;
        }

        public abstract void GotoNextState();

        public virtual void InitState()
        {
            foreach (var substate in substates.Values)
            {
                substate.Init();
            }
        }

        public HTAction Start()
        {
            CurSubstate.PreRun(game, curPlayer);
            return CurSubstate.RequestAction(game, curPlayer);
        }

        public HTAction Continue(object result)
        {
            object response = result;
            while (!stateEnded)
            {
                CurSubstate.HandleRequestAction(response, game, curPlayer);
                CurSubstate.SetNextSubstate(game, curPlayer);

                if (stateEnded)
                {
                    break;
                }

                CurSubstate.PreRun(game, curPlayer);
                HTAction a = CurSubstate.RequestAction(game, curPlayer);
                if (a != null)
                {
                    return a;
                }
                else
                {
                    response = null;
                }
            }

            cleanup();
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

        protected virtual void cleanup()
        {}

        protected abstract void initSubStates(GameState parent);
        protected abstract void copySubstates(GameState state, Game game);
    }
}
