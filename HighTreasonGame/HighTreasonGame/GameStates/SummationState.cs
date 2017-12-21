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

        // Copy constructor
        public SummationState(SummationState state, Game _game)
            : base(state, _game)
        {}

        public override void InitState()
        {
            foreach (var substate in substates.Values)
            {
                substate.Init();
            }

            if (game.StartState == this.StateType)
            {
                game.Board.Juries.RemoveRange(0, 6);

                game.Deck.DealCards(20).ForEach(c => game.Discards.MoveCard(c));

                game.Deck.DealCards(6).ForEach(c => game.GetPlayerOfSide(Player.PlayerSide.Prosecution).Hand.MoveCard(c));
                game.Deck.DealCards(6).ForEach(c => game.GetPlayerOfSide(Player.PlayerSide.Defense).Hand.MoveCard(c));
            }

            foreach (Player.PlayerSide side in new Player.PlayerSide[] { Player.PlayerSide.Prosecution, Player.PlayerSide.Defense })
            {
                Player player = game.GetPlayerOfSide(side);
                player.SummationDeck.MoveAllCardsToHolder(player.Hand);
            }

            curPlayer = game.GetPlayerOfSide(Player.PlayerSide.Prosecution);
            CurSubstate = substates[typeof(PlayerActionChoiceSubstate)];

            if (game.NotifyStateStart != null)
            {
                game.NotifyStateStart();
            }
        }

        public override void GotoNextState()
        {
            game.SetNextState(GameState.GameStateType.Deliberation);
        }
    }
}
