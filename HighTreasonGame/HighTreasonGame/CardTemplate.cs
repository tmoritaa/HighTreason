using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighTreasonGame
{
    public abstract class CardTemplate
    {
        public delegate void CardEffect(int gameId, BoardChoices choices);

        public delegate BoardChoices CardChoice(int gameId, IChoiceHandler choiceHandler);

        public string Name {
            get; private set;
        }
            
        public int ActionPts {
            get; private set;
        }

        public List<CardEffect> SelectionEvents {
            get; private set;
        }

        public List<CardChoice> SelectionEventChoices {
            get; private set;
        }

        public List<CardEffect> TrialEvents {
            get; private set;
        }

        public List<CardChoice> TrialEventChoices {
            get; private set;
        }

        public List<CardEffect> SummationEvents {
            get; private set;
        }

        public List<CardChoice> SummationEventChoices {
            get; private set;
        }

        public CardTemplate(string _name, int _actionPts)
        {
            Name = _name;
            ActionPts = _actionPts;

            SelectionEvents = new List<CardEffect>();
            TrialEvents = new List<CardEffect>();
            SummationEvents = new List<CardEffect>();

            SelectionEventChoices = new List<CardChoice>();
            TrialEventChoices = new List<CardChoice>();
            SummationEventChoices = new List<CardChoice>();

            addSelectionEventsAndChoices();
            addTrialEventsAndChoices();
            addSummationEventsAndChoices();
        }

        protected abstract void addSelectionEventsAndChoices();
        protected abstract void addTrialEventsAndChoices();
        protected abstract void addSummationEventsAndChoices();

        #region Search Utility
        protected EvidenceTrack findInsanityTrack(Game game)
        {
            List<HTGameObject> insanityTrack = game.GetHTGOFromCondition((HTGameObject htgo) =>
            {
                return (htgo.properties.Contains(Property.Insanity));
            });
            System.Diagnostics.Debug.Assert(insanityTrack.Count == 1, "Insanity track search failed");

            return (EvidenceTrack)insanityTrack[0];
        }

        protected EvidenceTrack findGuiltTrack(Game game)
        {
            List<HTGameObject> guiltTrack = game.GetHTGOFromCondition((HTGameObject htgo) =>
            {
                return (htgo.properties.Contains(Property.Guilt));
            });
            System.Diagnostics.Debug.Assert(guiltTrack.Count == 1, "Guilt track search failed");
            return (EvidenceTrack)guiltTrack[0];
        }

        #endregion

        #region Effect Utility
        protected void handleMomentOfInsight(Game game)
        {
            // TODO: implement.
        }
        #endregion
    }
}
