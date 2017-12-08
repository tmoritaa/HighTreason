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

        protected Player curPlayer;

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

        protected void passToNextPlayer()
        {
            curPlayer = game.GetOtherPlayer(curPlayer);
        }

        protected Jury chooseJuryChoice(List<Jury> juries, Player curPlayer, string desc)
        {
            Jury jury = null;
            while (jury == null)
            {
                BoardChoices boardChoices;
                curPlayer.ChoiceHandler.ChooseBoardObjects(
                    juries.Cast<BoardObject>().ToList(),
                    (Dictionary<BoardObject, int> selected) => { return true; },
                    (List<BoardObject> remainingChoices, Dictionary<BoardObject, int> selected) =>
                    {
                        return remainingChoices.Where(obj => !selected.ContainsKey(obj)).ToList();
                    },
                    (Dictionary<BoardObject, int> selected) => { return selected.Keys.Count == 1; },
                    game,
                    curPlayer,
                    desc,
                    out boardChoices);

                if (boardChoices.NotCancelled)
                {
                    jury = (Jury)boardChoices.SelectedObjs.Keys.First();
                }
            }

            return jury;
        }
    }
}
