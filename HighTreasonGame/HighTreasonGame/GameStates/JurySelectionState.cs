using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighTreasonGame.GameStates
{
    public class JurySelectionState : CardPlayState
    {
        public JurySelectionState(Game _game)
            : base(_game)
        {}

        public override void GotoNextState()
        {
            game.GotoStateAndStart(typeof(JuryDismissalState));
        }

        protected override void loopEndLogic(Game game)
        {
            base.loopEndLogic(game);

            // TODO: implement Jury dismissal.

            game.ShuffleDiscardBackToDeck();
        }
    }
}
