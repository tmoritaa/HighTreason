using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame.GameStates
{
    public class JuryDismissalState : GameState
    {
        public JuryDismissalState(Game _game) : base(_game)
        {}

        public override void StartState()
        {
            game.CurPlayer = game.GetPlayerOfSide(Player.PlayerSide.Prosecution);

            if (game.NotifyStateStart != null)
            {
                game.NotifyStateStart();
            }

            mainLoop();
        }

        private void mainLoop()
        {
            while (true)
            {
                if (game.NotifyStartOfTurn != null)
                {
                    game.NotifyStartOfTurn();
                }

                game.CurPlayer.DismissJury();

                if (game.Board.Juries.Count == GameConstants.NUM_JURY_AFTER_DISMISSAL)
                {
                    break;
                }

                game.PassToNextPlayer();
            }

            game.ShuffleDiscardBackToDeck();

            GotoNextState();
        }

        public override void GotoNextState()
        {
            game.SetNextState(typeof(TrialInChiefState));
        }
    }
}
