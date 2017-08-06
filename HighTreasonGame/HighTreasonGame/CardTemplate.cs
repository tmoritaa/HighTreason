using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HighTreasonGame.GameStates;

namespace HighTreasonGame
{
    public abstract class CardTemplate
    {
        public delegate void CardEffect(int gameId, BoardChoices choices);

        public delegate BoardChoices CardChoice(int gameId, IChoiceHandler choiceHandler);

        public string Name {
            get; private set;
        }

        // TODO: for now until we have enough cards.
        public void SetName(string name)
        {
            Name = name;
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

        public void PlayAsEvent(Type curStateType, int gameId, int idx, IChoiceHandler choiceHandler)
        {
            CardChoice cardChoice = null;
            CardEffect cardEffect = null;

            if (curStateType == typeof(JurySelectionState))
            {
                cardChoice = SelectionEventChoices[idx];
                cardEffect = SelectionEvents[idx];
            }
            else if (curStateType == typeof(TrialInChiefState))
            {
                cardChoice = TrialEventChoices[idx];
                cardEffect = TrialEvents[idx];
            }
            else if (curStateType == typeof(SummationState))
            {
                cardChoice = SummationEventChoices[idx];
                cardEffect = SummationEvents[idx];
            }

            System.Diagnostics.Debug.Assert(cardChoice != null && cardEffect != null, "Card choice or card effect is null. Should never happen");

            BoardChoices choices = cardChoice(gameId, choiceHandler);
            cardEffect(gameId, choices);
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
