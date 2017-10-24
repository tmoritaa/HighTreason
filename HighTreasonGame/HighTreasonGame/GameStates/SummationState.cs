using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame.GameStates
{
    public class SummationState : CardPlayState
    {
        public SummationState(Game _game)
            : base(GameStateType.Summation, _game)
        {}

        public override void StartState()
        {
            foreach (Player.PlayerSide side in new Player.PlayerSide[] { Player.PlayerSide.Prosecution, Player.PlayerSide.Defense })
            {
                Player player = game.GetPlayerOfSide(side);
                player.SummationDeck.MoveAllCardsToHolder(player.Hand);
            }

            game.CurPlayer = game.GetPlayerOfSide(Player.PlayerSide.Prosecution);

            if (game.NotifyStateStart != null)
            {
                game.NotifyStateStart();
            }

            mainLoop();
        }

        public override void GotoNextState()
        {
            game.SetNextState(GameState.GameStateType.Deliberation);
        }

        protected override void mainLoop()
        {
            while (true)
            {
                if (game.NotifyStartOfTurn != null)
                {
                    game.NotifyStartOfTurn();
                }

                game.CurPlayer.PerformPlayerAction();

                int numPlayersFinished = 0;
                List<Player> players = game.GetPlayers();
                players.ForEach(p => numPlayersFinished += (p.Hand.Cards.Count == 0) ? 1 : 0);
                if (numPlayersFinished == players.Count)
                {
                    break;
                }

                if ((game.CurPlayer.Side == Player.PlayerSide.Prosecution && game.CurPlayer.Hand.Cards.Count == 3) 
                    || game.CurPlayer.Hand.Cards.Count == 0)
                {
                    game.PassToNextPlayer();
                }
            }

            GotoNextState();
        }
    }
}
