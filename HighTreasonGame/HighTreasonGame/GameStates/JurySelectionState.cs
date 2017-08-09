﻿using System;
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

                game.CurPlayer.PlayCard(GetType());

                int numPlayersFinished = 0;
                List<Player> players = game.GetPlayers();
                players.ForEach(p => numPlayersFinished += p.MustPass() ? 1 : 0);
                if (numPlayersFinished == players.Count)
                {
                    break;
                }

                if (game.CurPlayer.MustPass())
                {
                    game.PassToNextPlayer();
                }
            }

            game.GetPlayers().ForEach(p => p.AddHandToSummation());

            GotoNextState();
        }
    }
}
