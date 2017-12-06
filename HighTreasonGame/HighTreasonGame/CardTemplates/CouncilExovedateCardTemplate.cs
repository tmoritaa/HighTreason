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
            SelectionEvents.Add(
                new CardEffectPair(genRevealOrPeakCardChoice(new HashSet<Property>(), 2, true, this.CardInfo.JurySelectionInfos[0].Description), 
                revealAllAspects));

            SelectionEvents.Add(
                new CardEffectPair(genRevealOrPeakCardChoice(new HashSet<Property>(), 1, false, this.CardInfo.JurySelectionInfos[1].Description), 
                peekAllAspects));
        }

        protected override void addTrialEventsAndChoices()
        {
            TrialEvents.Add(
                new CardEffectPair(
                    doNothingChoice,
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
                    }));

            TrialEvents.Add(
                new CardEffectPair(
                    doNothingChoice,
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
                    }));

            TrialEvents.Add(
                new CardEffectPair(
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
                    },
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
                    }));
        }

        protected override void addSummationEventsAndChoices()
        {
            SummationEvents.Add(
                new CardEffectPair(
                    doNothingChoice,
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
                    }));

            SummationEvents.Add(
                new CardEffectPair(
                    doNothingChoice,
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
                    }));

            SummationEvents.Add(
                new CardEffectPair(
                    doNothingChoice,
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
                    }));
        }
    }
}
