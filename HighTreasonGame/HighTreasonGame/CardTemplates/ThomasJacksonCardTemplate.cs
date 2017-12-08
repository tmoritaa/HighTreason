using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame
{
    [CardTemplateAttribute]
    public class ThomasJacksonCardTemplate : CardTemplate
    {
        public ThomasJacksonCardTemplate()
            : base("Thomas E. Jackson", 2, Player.PlayerSide.Prosecution)
        { }

        protected override void addSelectionEventsAndChoices()
        {
            SelectionEvents.Add(
                new CardEffectPair(
                    genRevealOrPeakCardChoice(new HashSet<Property>() { Property.Language }, 4, true, this.CardInfo.JurySelectionInfos[0].Description),
                    revealAllAspects));

            SelectionEvents.Add(
                new CardEffectPair(
                    genRevealOrPeakCardChoice(new HashSet<Property>() { Property.Occupation }, 3, true, this.CardInfo.JurySelectionInfos[1].Description),
                    revealAllAspects));
        }

        protected override void addTrialEventsAndChoices()
        {
            TrialEvents.Add(
                new CardEffectPair(
                    genAspectTrackForModCardChoice(new HashSet<Property>(), 1, 1, false, this.CardInfo.TrialInChiefInfos[0].Description),
                    raiseGuiltAndOneAspectEffect));

            TrialEvents.Add(
                new CardEffectPair(
                    doNothingChoice,
                    (Game game, Player choosingPlayer, BoardChoices choices) =>
                    {
                        findAspectTracksWithProp(game, Property.English)[0].AddToValue(2);
                    }));

            TrialEvents.Add(
                new CardEffectPair(
                    doNothingChoice,
                    (Game game, Player choosingPlayer, BoardChoices choices) =>
                    {
                        int modval = calcModValueBasedOnSide(1, choosingPlayer);
                        findAspectTracksWithProp(game, Property.GovWorker, Property.Farmer).ForEach(t => t.AddToValue(modval));
                    }));
        }

        protected override void addSummationEventsAndChoices()
        {
            SummationEvents.Add(
                new CardEffectPair(
                    doNothingChoice,
                    (Game game, Player choosingPlayer, BoardChoices choices) =>
                    {
                        List<AspectTrack> tracks = findAspectTracksWithProp(game, Property.English, Property.GovWorker, Property.Farmer, Property.Merchant);

                        foreach (AspectTrack t in tracks)
                        {
                            if (t.Properties.Contains(Property.English) || t.Properties.Contains(Property.GovWorker))
                            {
                                t.AddToValue(1);
                            }
                            else if (t.Properties.Contains(Property.Farmer))
                            {
                                t.AddToValue(-1);
                            }
                            else if (t.Properties.Contains(Property.Merchant))
                            {
                                t.AddToValue(calcModValueBasedOnSide(2, choosingPlayer));
                            }
                        }
                    }));
        }
    }
}
