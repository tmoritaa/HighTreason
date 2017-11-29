using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json.Linq;

namespace HighTreasonGame.CardTemplates
{
    public class CouncilExovedateCardTemplate : CardTemplate
    {
        public CouncilExovedateCardTemplate(JObject json)
            : base("Council of the Exovedate", 2, json)
        {}

        protected override void addSelectionEventsAndChoices()
        {
            SelectionEventChoices.Add(genRevealOrPeakCardChoice(new HashSet<Property>(), 2, true, this.CardInfo.JurySelectionPairs[0].Description));
            SelectionEvents.Add(revealAllAspects);

            SelectionEventChoices.Add(genRevealOrPeakCardChoice(new HashSet<Property>(), 1, false, this.CardInfo.JurySelectionPairs[1].Description));
            SelectionEvents.Add(peekAllAspects);
        }

        protected override void addTrialEventsAndChoices()
        {
            TrialEventChoices.Add(doNothingChoice);

            TrialEvents.Add(
                (Game game, BoardChoices choices) =>
                {
                    int modValue = calcModValueBasedOnSide(2, game);

                    AspectTrack aspectTrack = (AspectTrack)game.FindBO(
                        (BoardObject htgo) =>
                        {
                            return (htgo.Properties.Contains(Property.Track)
                            && htgo.Properties.Contains(Property.Aspect)
                            && htgo.Properties.Contains(Property.French));
                        })[0];

                    aspectTrack.AddToValue(modValue);
                });

            TrialEventChoices.Add(doNothingChoice);

            TrialEvents.Add(
                (Game game, BoardChoices choices) =>
                {
                    int modValue = calcModValueBasedOnSide(2, game);

                    AspectTrack aspectTrack = (AspectTrack)game.FindBO(
                        (BoardObject htgo) =>
                        {
                            return (htgo.Properties.Contains(Property.Track)
                            && htgo.Properties.Contains(Property.Aspect)
                            && htgo.Properties.Contains(Property.English));
                        })[0];

                    aspectTrack.AddToValue(modValue);
                });

            TrialEventChoices.Add(
                (Game game, ChoiceHandler choiceHandler) =>
                {
                    BoardChoices choices = new BoardChoices();
                    
                    AspectTrack aspectTrack = (AspectTrack)game.FindBO(
                        (BoardObject htgo) =>
                        {
                            return (htgo.Properties.Contains(Property.Track)
                            && htgo.Properties.Contains(Property.Aspect)
                            && htgo.Properties.Contains(Property.Merchant));
                        })[0];

                    choices.SelectedObjs.Keys.Cast<AspectTrack>().ToList().Add(aspectTrack);

                    return choices;
                });

            TrialEvents.Add(
                (Game game, BoardChoices choices) =>
                {
                    int modValue = calcModValueBasedOnSide(1, game);

                    AspectTrack aspectTrack = (AspectTrack)game.FindBO(
                        (BoardObject htgo) =>
                        {
                            return (htgo.Properties.Contains(Property.Track)
                            && htgo.Properties.Contains(Property.Aspect)
                            && htgo.Properties.Contains(Property.Merchant));
                        })[0];

                    aspectTrack.AddToValue(modValue);
                });
        }

        protected override void addSummationEventsAndChoices()
        {
            SummationEventChoices.Add(doNothingChoice);
            SummationEvents.Add(
                (Game game, BoardChoices choices) =>
                {
                    int modValue = calcModValueBasedOnSide(2, game);

                    AspectTrack aspectTrack = (AspectTrack)game.FindBO(
                        (BoardObject htgo) =>
                        {
                            return (htgo.Properties.Contains(Property.Track)
                            && htgo.Properties.Contains(Property.Aspect)
                            && htgo.Properties.Contains(Property.French));
                        })[0];

                    aspectTrack.AddToValue(modValue);
                });

            SummationEventChoices.Add(doNothingChoice);
            SummationEvents.Add(
                (Game game, BoardChoices choices) =>
                {
                    int modValue = calcModValueBasedOnSide(2, game);

                    AspectTrack aspectTrack = (AspectTrack)game.FindBO(
                        (BoardObject htgo) =>
                        {
                            return (htgo.Properties.Contains(Property.Track)
                            && htgo.Properties.Contains(Property.Aspect)
                            && htgo.Properties.Contains(Property.English));
                        })[0];

                    aspectTrack.AddToValue(modValue);
                });

            SummationEventChoices.Add(doNothingChoice);
            SummationEvents.Add(
                (Game game, BoardChoices choices) =>
                {
                    int modValue = calcModValueBasedOnSide(1, game);

                    AspectTrack aspectTrack = (AspectTrack)game.FindBO(
                        (BoardObject htgo) =>
                        {
                            return (htgo.Properties.Contains(Property.Track)
                            && htgo.Properties.Contains(Property.Aspect)
                            && htgo.Properties.Contains(Property.Merchant));
                        })[0];

                    aspectTrack.AddToValue(modValue);
                });
        }
    }
}
