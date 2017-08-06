using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighTreasonGame.GameStates
{
    public class TrialInChiefState : CardPlayState
    {
        private int numVisits = 0;

        public TrialInChiefState(int _gameId) 
            : base(_gameId)
        {}

        public override void StartState()
        {
            numVisits += 1;
            base.StartState();
        }

        public override void GotoNextState()
        {
            Type nextStateType = (numVisits < 2) ? typeof(TrialInChiefState) : typeof(SummationState);
            Game.GetGameFromId(gameId).GotoStateAndStart(nextStateType);
        }
    }
}
