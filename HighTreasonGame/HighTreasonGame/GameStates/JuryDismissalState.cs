using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame.GameStates
{
    public class JuryDismissalState : GameState
    {
        public JuryDismissalState(Game _game) 
            : base(GameStateType.JuryDismissal, _game)
        {}

        public override void StartState()
        {
            curPlayer = game.GetPlayerOfSide(Player.PlayerSide.Prosecution);

            if (game.NotifyStateStart != null)
            {
                game.NotifyStateStart();
            }

            mainLoop();
        }

        public override void GotoNextState()
        {
            game.SetNextState(GameState.GameStateType.TrialInChief);
        }

        private void mainLoop()
        {
            while (true)
            {
                if (game.NotifyStartOfTurn != null)
                {
                    game.NotifyStartOfTurn();
                }

                dismissJury(curPlayer);

                if (game.Board.Juries.Count == GameConstants.NUM_JURY_AFTER_DISMISSAL)
                {
                    break;
                }

                passToNextPlayer();
            }

            shuffleDiscardBackToDeck();

            GotoNextState();
        }

        private void dismissJury(Player curPlayer)
        {
            Jury jury = chooseJuryChoice(game.Board.Juries, curPlayer, "Select Jury to Dismiss");

            FileLogger.Instance.Log(curPlayer + " dismissed " + jury);

            game.Board.RemoveJury(jury);
        }

        private void shuffleDiscardBackToDeck()
        {
            game.Discards.MoveAllCardsToHolder(game.Deck);
            game.Deck.Shuffle();
        }
    }
}
