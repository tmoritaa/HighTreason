using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json.Linq;

namespace HighTreasonGame.CardTemplates
{
    public class HaroldRossCardTemplate : CardTemplate
    {
        public HaroldRossCardTemplate(JObject json)
            : base("Harold Ross", 3, json)
        { }

        protected override void addSelectionEventsAndChoices()
        {
            SelectionEvents.Add(
                new CardEffectPair(
                    genRevealOrPeakCardChoice(new HashSet<Property>() { Property.Occupation }, 3, true, this.CardInfo.JurySelectionPairs[0].Description),
                    revealAllAspects));

            SelectionEvents.Add(
                new CardEffectPair(
                    genRevealOrPeakCardChoice(new HashSet<Property>() { Property.Language }, 4, true, this.CardInfo.JurySelectionPairs[1].Description),
                    revealAllAspects));
        }

        protected override void addTrialEventsAndChoices()
        {
            TrialEvents.Add(
                new CardEffectPair(
                    genAspectTrackForModCardChoice(new HashSet<Property>(), 1, 1, false, this.CardInfo.TrialInChiefPairs[0].Description),
                    (Game game, BoardChoices choices) =>
                    {
                        game.GetGuiltTrack().AddToValue(1);
                        choices.SelectedObjs.Keys.Cast<AspectTrack>().ToList().ForEach(t => t.AddToValue(1));
                    }));

            TrialEvents.Add(
                new CardEffectPair(
                    genAspectTrackForModCardChoice(new HashSet<Property>(), 1, 2, true, this.CardInfo.TrialInChiefPairs[1].Description),
                    (Game game, BoardChoices choices) =>
                    {
                        int modValue = calcModValueBasedOnSide(2, game);
                        choices.SelectedObjs.Keys.Cast<AspectTrack>().ToList().ForEach(t => t.AddToValue(modValue));
                    }));
        }

        protected override void addSummationEventsAndChoices()
        {
            SummationEvents.Add(
                new CardEffectPair(
                    doNothingChoice,
                    (Game game, BoardChoices choices) =>
                    {
                        List<BoardObject> options = game.FindBO(
                            (BoardObject htgo) =>
                            {
                                return (htgo.Properties.Contains(Property.Track)
                                && htgo.Properties.Contains(Property.Aspect)
                                && (htgo.Properties.Contains(Property.English) || htgo.Properties.Contains(Property.GovWorker)));
                            });

                        options.Cast<AspectTrack>().ToList().ForEach(t => t.AddToValue(1));
                    }));

            SummationEvents.Add(
                new CardEffectPair(
                    doNothingChoice,
                    (Game game, BoardChoices choices) =>
                    {
                        List<BoardObject> options = game.FindBO(
                            (BoardObject htgo) =>
                            {
                                return (htgo.Properties.Contains(Property.Track)
                                && htgo.Properties.Contains(Property.Aspect)
                                && (htgo.Properties.Contains(Property.Farmer) || htgo.Properties.Contains(Property.French)));
                            });

                        options.Cast<AspectTrack>().ToList().ForEach(t => t.AddToValue(-1));
                    }));
        }
    }
}
