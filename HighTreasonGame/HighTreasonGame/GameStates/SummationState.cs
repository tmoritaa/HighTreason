using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame.GameStates
{
    public class SummationState : CardPlayState
    {
        public SummationState(Game _game)
            : base(_game)
        {}

        public override void GotoNextState()
        {
            throw new NotImplementedException();
        }
    }
}
