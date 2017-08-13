using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame.GameStates
{
    public class SummationState : CardPlayState
    {
        public SummationState(Game _game)
            : base(_game)
        {}

        public override void StartState()
        {
            foreach (Player.PlayerSide side in new Player.PlayerSide[] { Player.PlayerSide.Prosecution, Player.PlayerSide.Defense })
            {
                Player player = game.GetPlayerOfSide(side);
                player.SetupHand(player.SummationDeck.DealoutCards());
            }

            game.CurPlayer = game.GetPlayerOfSide(Player.PlayerSide.Prosecution);

            mainLoop();
        }

        public override void GotoNextState()
        {
            game.SetNextState(typeof(DelibrationState));
        }

        protected override void mainLoop()
        {
            while (true)
            {
                game.EventHandler.StartOfNewTurn(game, this.GetType());

                game.CurPlayer.PlayCard();

                int numPlayersFinished = 0;
                List<Player> players = game.GetPlayers();
                players.ForEach(p => numPlayersFinished += (p.Hand.Count == 0) ? 1 : 0);
                if (numPlayersFinished == players.Count)
                {
                    break;
                }

                if ((game.CurPlayer.Side == Player.PlayerSide.Prosecution && game.CurPlayer.Hand.Count == 3) 
                    || game.CurPlayer.Hand.Count == 0)
                {
                    game.PassToNextPlayer();
                }
            }

            GotoNextState();
        }
    }
}
