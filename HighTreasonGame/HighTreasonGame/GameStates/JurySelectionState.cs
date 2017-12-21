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

        // Copy constructor
        public JurySelectionState(JurySelectionState state, Game _game)
            : base(state, _game)
        {}

        public override void GotoNextState()
        {
            game.SetNextState(GameState.GameStateType.JuryDismissal);
        }
    }
}
