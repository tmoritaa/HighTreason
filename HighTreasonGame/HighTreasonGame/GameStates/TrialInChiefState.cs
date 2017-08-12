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
            : base(_game)
        {}

        public override void StartState()
        {
            numVisits += 1;
            base.StartState();
        }

        public override void GotoNextState()
        {
            Type nextStateType = (numVisits < 2) ? typeof(TrialInChiefState) : typeof(SummationState);
            game.setNextState(nextStateType);
        }

        protected override void mainLoop()
        {
            while (true)
            {
                game.EventHandler.StartOfNewTurn(game, this.GetType());

                game.CurPlayer.PlayCard(GetType());

                int numPlayersFinished = 0;
                List<Player> players = game.GetPlayers();
                players.ForEach(p => numPlayersFinished += (p.Hand.Count == 2) ? 1 : 0);
                if (numPlayersFinished == players.Count)
                {
                    break;
                }

                if (game.GetOtherPlayer().Hand.Count > 2)
                {
                    game.PassToNextPlayer();
                }
            }

            game.GetPlayers().ForEach(p => p.AddHandToSummation());

            GotoNextState();
        }
    }
}
