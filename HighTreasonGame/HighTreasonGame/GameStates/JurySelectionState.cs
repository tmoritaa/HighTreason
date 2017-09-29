using System;
using System.Collections.Generic;

namespace HighTreasonGame.GameStates
{
    public class JurySelectionState : CardPlayState
    {
        public JurySelectionState(Game _game)
            : base(GameStateType.JurySelection, _game)
        {}

        public override void GotoNextState()
        {
            game.SetNextState(GameState.GameStateType.JuryDismissal);
        }

        protected override void mainLoop()
        {
            while (true)
            {
                if (game.NotifyStartOfTurn != null)
                {
                    game.NotifyStartOfTurn();
                }

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
