using System;
using System.Collections.Generic;
using System.Linq;

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
    }
}
