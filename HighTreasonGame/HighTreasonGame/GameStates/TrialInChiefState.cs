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

        public override void InitState()
        {
            if (game.StartState == this.StateType)
            {
                game.Board.Juries.RemoveRange(0, 6);

                game.Deck.DealCards(2).ForEach(c => game.GetPlayerOfSide(Player.PlayerSide.Prosecution).SummationDeck.MoveCard(c));
                game.Deck.DealCards(2).ForEach(c => game.GetPlayerOfSide(Player.PlayerSide.Defense).SummationDeck.MoveCard(c));
            }

            numVisits += 1;
            base.InitState();
        }

        public override void GotoNextState()
        {
            GameState.GameStateType nextStateType = (numVisits < 2) ? GameState.GameStateType.TrialInChief : GameState.GameStateType.Summation;
            game.SetNextState(nextStateType);
        }
    }
}
