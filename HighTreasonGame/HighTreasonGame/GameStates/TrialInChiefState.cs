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
            int numPlayersFinished = 0;
            while (numPlayersFinished < 2)
            {
                if (game.NotifyStartOfTurn != null)
                {
                    game.NotifyStartOfTurn();
                }

                game.CurPlayer.PerformPlayerAction();

                if (game.CurPlayer.Hand.Cards.Count == 2)
                {
                    numPlayersFinished += 1;
                    game.CurPlayer.AddHandToSummation();
                }

                if (game.GetOtherPlayer(game.CurPlayer).Hand.Cards.Count > 2)
                {
                    game.PassToNextPlayer();
                }
            }

            GotoNextState();
        }
    }
}
