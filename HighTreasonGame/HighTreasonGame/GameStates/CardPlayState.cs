using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame.GameStates
{
    public abstract class CardPlayState : GameState
    {
        public CardPlayState(GameStateType _stateType, Game _game) 
            : base(_stateType, _game)
        {}

        public override void StartState()
        {
            foreach (Player.PlayerSide side in new Player.PlayerSide[] { Player.PlayerSide.Prosecution, Player.PlayerSide.Defense })
            {
                game.GetPlayerOfSide(side).SetupHand(game.Deck.DealCards(GameConstants.NUM_HAND_SIZE));
            }

            game.CurPlayer = game.GetPlayerOfSide(Player.PlayerSide.Prosecution);

            if (game.NotifyStateStart != null)
            {
                game.NotifyStateStart();
            }

            mainLoop();
        }

        protected abstract void mainLoop();
    }
}
