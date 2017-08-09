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

            mainLoop();
        }

        private void mainLoop()
        {
            while (true)
            {
                game.EventHandler.StartOfNewTurn(game, this.GetType());

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
            game.setNextState(typeof(TrialInChiefState));
        }
    }
}
