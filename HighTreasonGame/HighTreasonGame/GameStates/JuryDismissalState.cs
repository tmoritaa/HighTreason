using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighTreasonGame.GameStates
{
    public class JuryDismissalState : GameState
    {
        public JuryDismissalState(int _gameId) : base(_gameId)
        {}

        public override void StartState()
        {
            Game game = Game.GetGameFromId(gameId);

            game.CurPlayer = game.GetPlayerOfSide(Player.PlayerSide.Prosecution);

            mainLoop();
        }

        private void mainLoop()
        {
            Game game = Game.GetGameFromId(gameId);

            while (true)
            {
                System.Console.WriteLine(game);

                game.CurPlayer.DismissJury();

                if (game.Board.Juries.Count == GameConstants.NUM_JURY_AFTER_DISMISSAL)
                {
                    break;
                }

                game.PassToNextPlayer();
            }

            GotoNextState();
        }

        public override void GotoNextState()
        {
            Game.GetGameFromId(gameId).GotoStateAndStart(typeof(TrialInChiefState));
        }
    }
}
