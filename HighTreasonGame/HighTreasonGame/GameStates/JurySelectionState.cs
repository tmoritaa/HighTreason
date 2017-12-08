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
            int numPlayersFinished = 0;
            while (numPlayersFinished < game.GetPlayers().Count)
            {
                if (game.NotifyStartOfTurn != null)
                {
                    game.NotifyStartOfTurn();
                }

                performPlayerAction(curPlayer);

                if (curPlayer.Hand.Cards.Count == 2)
                {
                    numPlayersFinished += 1;
                    curPlayer.AddHandToSummation();
                    passToNextPlayer();
                }
            }
            
            GotoNextState();
        }
    }
}
