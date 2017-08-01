using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighTreasonGame
{
    public class CardTemplate
    {
        public delegate void CardEvent(int gameId, IChoiceHandler choiceHandler);

        public string Name {
            get; private set;
        }
            
        public int ActionPts {
            get; private set;
        }

        public CardEvent SelectionEvent {
            get; private set;
        }

        public CardEvent TrialEvent {
            get; private set;
        }
        public CardEvent SummationEvent {
            get; private set;
        }

        public CardTemplate(string _name, int _actionPts, CardEvent _selectionEvent, CardEvent _trialEvent, CardEvent _summationEvent)
        {
            Name = _name;
            ActionPts = _actionPts;
            SelectionEvent = _selectionEvent;
            TrialEvent = _trialEvent;
            SummationEvent = _summationEvent;
        }
    }
}
