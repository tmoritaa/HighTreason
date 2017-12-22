using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame.GameStates
{
    public class JuryDismissalState : GameState
    {
        private class DismissJurySubstate : GameSubState
        {
            private BoardChoices boardChoices;

            public DismissJurySubstate(GameState _parent) : base(_parent)
            {
            }

            // Copy constructor
            public DismissJurySubstate(DismissJurySubstate substate, GameState parentState, Game game) : base(parentState)
            {}
            
            public override void PreRun(Game game, Player curPlayer)
            {
                boardChoices = null;
            }

            public override Action RequestAction(Game game, Player curPlayer)
            {
                return
                    new Action(
                        ChoiceHandler.ChoiceType.BoardObjects,
                        curPlayer.ChoiceHandler,
                        game.Board.Juries.Cast<BoardObject>().ToList(),
                        (Func<Dictionary<BoardObject, int>, bool>)((Dictionary<BoardObject, int> selected) => { return true; }),
                        (Func<List<BoardObject>, Dictionary<BoardObject, int>, List<BoardObject>>)((List<BoardObject> remainingChoices, Dictionary<BoardObject, int> selected) =>
                        {
                            return remainingChoices.Where(obj => !selected.ContainsKey(obj)).ToList();
                        }),
                        (Func<Dictionary<BoardObject, int>, bool>)((Dictionary<BoardObject, int> selected) => { return selected.Keys.Count == 1; }),
                        game,
                        curPlayer,
                        "Select Jury to Dismiss");
            }

            public override void HandleRequestAction(Action action, Game game, Player curPlayer)
            {
                boardChoices = new BoardChoices((BoardChoices)action.ChoiceResult, game);

                if (boardChoices.NotCancelled)
                {
                    Jury juryToDismiss = (Jury)boardChoices.SelectedObjs.Keys.First();
                    FileLogger.Instance.Log(curPlayer + " dismissed " + juryToDismiss);
                    game.Board.RemoveJury(juryToDismiss);

                    parentState.PassToNextPlayer();
                }

                if (game.Board.Juries.Count == GameConstants.NUM_JURY_AFTER_DISMISSAL)
                {
                    shuffleDiscardBackToDeck(game);
                }
            }

            public override void SetNextSubstate(Game game, Player curPlayer)
            {
                if (game.Board.Juries.Count == GameConstants.NUM_JURY_AFTER_DISMISSAL)
                {
                    parentState.SignifyStateEnd();
                }
            }

            private void shuffleDiscardBackToDeck(Game game)
            {
                game.Discards.MoveAllCardsToHolder(game.Deck);
                game.Deck.Shuffle();
            }
        }

        public JuryDismissalState(Game _game) 
            : base(GameStateType.JuryDismissal, _game)
        {}

        public JuryDismissalState(JuryDismissalState state, Game _game)
            : base(state, _game)
        {}

        public override void InitState()
        {
            base.InitState();

            curPlayer = game.GetPlayerOfSide(Player.PlayerSide.Prosecution);

            CurSubstate = substates[typeof(DismissJurySubstate)];

            if (game.NotifyStateStart != null)
            {
                game.NotifyStateStart();
            }
        }

        public override void GotoNextState()
        {
            game.SetNextState(GameState.GameStateType.TrialInChief);
        }

        protected override void initSubStates(GameState parent)
        {
            substates.Add(typeof(DismissJurySubstate), new DismissJurySubstate(this));
        }

        protected override void copySubstates(GameState state, Game game)
        {
            Type substateType = typeof(DismissJurySubstate);
            substates[substateType] = new DismissJurySubstate((DismissJurySubstate)((JuryDismissalState)state).substates[substateType], this, game);
        }
    }
}
