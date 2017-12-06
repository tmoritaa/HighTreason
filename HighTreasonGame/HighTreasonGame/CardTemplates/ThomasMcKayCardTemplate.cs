using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json.Linq;

namespace HighTreasonGame.CardTemplates
{
    public class ThomasMcKayCardTemplate : CardTemplate
    {
        public ThomasMcKayCardTemplate(JObject json)
            : base("Thomas McKay", 2, json)
        { }

        protected override void addSelectionEventsAndChoices()
        {
            SelectionEvents.Add(
                new CardEffectPair(
                    genRevealOrPeakCardChoice(new HashSet<Property>(), 2, true, this.CardInfo.JurySelectionInfos[0].Description,
                        null,
                        (List<BoardObject> remainingChoices, Dictionary<BoardObject, int> selected) =>
                        {
                            List<BoardObject> newChoices = new List<BoardObject>(remainingChoices);
                            foreach (BoardObject obj in selected.Keys)
                            {
                                if (obj.Properties.Contains(Property.Religion))
                                {
                                    newChoices = newChoices.Where(c => !c.Properties.Contains(Property.Religion)).ToList();
                                }

                                if (obj.Properties.Contains(Property.Occupation))
                                {
                                    newChoices = newChoices.Where(c => !c.Properties.Contains(Property.Occupation)).ToList();
                                }

                                if (obj.Properties.Contains(Property.Language))
                                {
                                    newChoices = newChoices.Where(c => !c.Properties.Contains(Property.Language)).ToList();
                                }
                            }

                            return newChoices;
                        }),
                    revealAllAspects));
        }

        protected override void addTrialEventsAndChoices()
        {
            TrialEvents.Add(
                new CardEffectPair(
                    genAspectTrackForModCardChoice(new HashSet<Property>(), 1, 1, false, this.CardInfo.TrialInChiefInfos[0].Description),
                    raiseGuiltAndOneAspectEffect));
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
                                    && htgo.Properties.Contains(Property.English));
                                });

                        options.Cast<AspectTrack>().ToList().ForEach(t => t.AddToValue(2));
                    }));
        }
    }
}
