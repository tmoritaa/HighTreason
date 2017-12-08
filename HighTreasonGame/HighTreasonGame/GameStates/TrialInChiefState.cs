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
            if (game.StartState == this.StateType)
            {
                game.Board.Juries.RemoveRange(0, 6);

                game.Deck.DealCards(2).ForEach(c => game.GetPlayerOfSide(Player.PlayerSide.Prosecution).SummationDeck.MoveCard(c));
                game.Deck.DealCards(2).ForEach(c => game.GetPlayerOfSide(Player.PlayerSide.Defense).SummationDeck.MoveCard(c));
            }

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

                performPlayerAction(curPlayer);

                if (curPlayer.Hand.Cards.Count == 2)
                {
                    numPlayersFinished += 1;
                    curPlayer.AddHandToSummation();
                }

                if (game.GetOtherPlayer(curPlayer).Hand.Cards.Count > 2)
                {
                    passToNextPlayer();
                }
            }

            GotoNextState();
        }
    }
}
