using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame.GameStates
{
    public class TrialInChiefState : CardPlayState
    {
        private int numVisits = 0;

        public TrialInChiefState(Game _game) 
            : base(GameStateType.TrialInChief, _game)
        {}

        public override void StartState()
        {
            numVisits += 1;
            base.StartState();
        }

        public override void GotoNextState()
        {
            GameState.GameStateType nextStateType = (numVisits < 2) ? GameState.GameStateType.TrialInChief : GameState.GameStateType.Summation;
            game.SetNextState(nextStateType);
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
                players.ForEach(p => numPlayersFinished += (p.Hand.Cards.Count == 2) ? 1 : 0);
                if (numPlayersFinished == players.Count)
                {
                    break;
                }

                if (game.GetOtherPlayer().Hand.Cards.Count > 2)
                {
                    game.PassToNextPlayer();
                }
            }

            game.GetPlayers().ForEach(p => p.AddHandToSummation());

            GotoNextState();
        }
    }
}
