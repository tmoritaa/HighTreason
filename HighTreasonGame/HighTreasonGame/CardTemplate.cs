using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighTreasonGame
{
    public abstract class CardTemplate
    {
        public delegate void CardEvent(int gameId, IChoiceHandler choiceHandler);

        public string Name {
            get; private set;
        }
            
        public int ActionPts {
            get; private set;
        }

        public List<CardEvent> SelectionEvents {
            get; private set;
        }

        public List<CardEvent> TrialEvents {
            get; private set;
        }
        public List<CardEvent> SummationEvents {
            get; private set;
        }

        public CardTemplate(string _name, int _actionPts)
        {
            Name = _name;
            ActionPts = _actionPts;

            SelectionEvents = new List<CardEvent>();
            TrialEvents = new List<CardEvent>();
            SummationEvents = new List<CardEvent>();

            addSelectionEvents();
            addTrialEvents();
            addSummationEvents();
        }

        protected abstract void addSelectionEvents();
        protected abstract void addTrialEvents();
        protected abstract void addSummationEvents();

        protected void handleMomentOfInsight()
        {
            // TODO: implement.
        }

        protected void handleGuilt(Game game, int modValue)
        {
            List<HTGameObject> guiltTrack = game.GetHTGOFromCondition((HTGameObject htgo) =>
            {
                return (htgo.properties.Contains(Property.Guilt));
            });
            System.Diagnostics.Debug.Assert(guiltTrack.Count == 1, "Guilt track search failed");
            ((EvidenceTrack)guiltTrack[0]).AddToValue(modValue);
        }

        protected void handleInsanity(Game game, int modValue)
        {
            List<HTGameObject> insanityTrack = game.GetHTGOFromCondition((HTGameObject htgo) =>
            {
                return (htgo.properties.Contains(Property.Insanity));
            });
            System.Diagnostics.Debug.Assert(insanityTrack.Count == 1, "Insanity track search failed");
            ((EvidenceTrack)insanityTrack[0]).AddToValue(modValue);
        }

        protected void handleAspectTrackChange(Game game, IChoiceHandler choiceHandler, int numChoices, int modValue, Func<HTGameObject, bool> condition)
        {
            List<HTGameObject> choices = game.GetHTGOFromCondition(condition);

            List<AspectTrack> tracks = new List<AspectTrack>();
            if (choices.Count > 1)
            {
                System.Diagnostics.Debug.Assert(numChoices > 0, "Number of choices is not zero");
                tracks = choiceHandler.ChooseAspectTracks(choices, numChoices);
            }
            else
            {
                tracks.Add((AspectTrack)choices[0]);
            }

            tracks.ForEach(t => t.AddToValue(modValue));
        }
    }
}
