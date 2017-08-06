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
            game.GotoStateAndStart(nextStateType);
        }
    }
}
