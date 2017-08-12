using System;
using System.Collections.Generic;

namespace HighTreasonGame.GameStates
{
    public class JurySelectionState : CardPlayState
    {
        public JurySelectionState(Game _game)
            : base(_game)
        {}

        public override void GotoNextState()
        {
            game.setNextState(typeof(JuryDismissalState));
        }

        protected override void mainLoop()
        {
            while (true)
            {
                game.EventHandler.StartOfNewTurn(game, this.GetType());

                game.CurPlayer.PlayCard();

                int numPlayersFinished = 0;
                List<Player> players = game.GetPlayers();
                players.ForEach(p => numPlayersFinished += (p.Hand.Count == 2) ? 1 : 0);
                if (numPlayersFinished == players.Count)
                {
                    break;
                }

                if (game.CurPlayer.Hand.Count == 2)
                {
                    game.PassToNextPlayer();
                }
            }

            game.GetPlayers().ForEach(p => p.AddHandToSummation());

            GotoNextState();
        }
    }
}
