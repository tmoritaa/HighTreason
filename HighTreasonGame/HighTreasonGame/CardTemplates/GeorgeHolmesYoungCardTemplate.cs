using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json.Linq;

namespace HighTreasonGame.CardTemplates
{
    public class GeorgeHolmesYoungCardTemplate : CardTemplate
    {
        public GeorgeHolmesYoungCardTemplate(JObject json)
            : base("George Holmes Young", 2, json)
        { }

        protected override void addSelectionEventsAndChoices()
        {
            SelectionEvents.Add(
                new CardEffectPair(genRevealOrPeakCardChoice(new HashSet<Property>() { Property.Occupation }, 2, true, this.CardInfo.JurySelectionInfos[0].Description),
                revealAllAspects));

            SelectionEvents.Add(
                new CardEffectPair(genRevealOrPeakCardChoice(new HashSet<Property>() { Property.Occupation }, 1, true, this.CardInfo.JurySelectionInfos[1].Description),
                peekAllAspects));
        }

        protected override void addTrialEventsAndChoices()
        {
            TrialEvents.Add(
                new CardEffectPair(
                    doNothingChoice,
                    (Game game, BoardChoices choices) =>
                    {
                        raiseGuiltAndOneAspectEffect(game, choices);
                        game.OfficersRecalledPlayable = true;
                    }));
        }

        protected override void addSummationEventsAndChoices()
        {
            SummationEvents.Add(
                new CardEffectPair(
                    doNothingChoice,
                    (Game game, BoardChoices choices) =>
                    {
                        List<AspectTrack> aspectTracks = game.FindBO(
                            (BoardObject htgo) =>
                            {
                                return (htgo.Properties.Contains(Property.Track)
                                && htgo.Properties.Contains(Property.Aspect)
                                && htgo.Properties.Contains(Property.Occupation));
                            }).Cast<AspectTrack>().ToList();

                        aspectTracks.ForEach(t => t.AddToValue(1));

                        game.OfficersRecalledPlayable = true;
                    }));
        }
    }
}
