using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json.Linq;

namespace HighTreasonGame
{
    /*
    [CardTemplateAttribute]
    public class WilliamTompkinsCardTemplate : CardTemplate
    {
        public WilliamTompkinsCardTemplate()
            : base("William Tompkins", 3, Player.PlayerSide.Prosecution)
        { }

        protected override void addSelectionEventsAndChoices()
        {
            SelectionEvents.Add(
                new CardEffectPair(
                    genRevealOrPeakCardChoice(new HashSet<Property> { Property.Occupation }, 3, true, this.CardInfo.JurySelectionInfos[0].Description),
                    revealAllAspects));

            SelectionEvents.Add(
                new CardEffectPair(
                    genRevealOrPeakCardChoice(new HashSet<Property> { Property.Language }, 2, true, this.CardInfo.JurySelectionInfos[1].Description),
                    revealAllAspects));

            SelectionEvents.Add(
                new CardEffectPair(
                    genRevealOrPeakCardChoice(new HashSet<Property>() { Property.Religion }, 1, true, this.CardInfo.JurySelectionInfos[2].Description),
                    revealAllAspects));
        }

        protected override void addTrialEventsAndChoices()
        {
            TrialEvents.Add(
                new CardEffectPair(
                    (Game game, Player choosingPlayer, ChoiceHandler choiceHandler) =>
                    {
                        CardChoice pickAspectFunc = genAspectTrackForModCardChoice(new HashSet<Property>() { Property.Occupation }, 1, 2, true, this.CardInfo.TrialInChiefInfos[0].Description);
                        BoardChoices boardChoices = pickAspectFunc(game, choosingPlayer, choiceHandler);

                        if (boardChoices.NotCancelled)
                        {
                            boardChoices.NotCancelled = handleMomentOfInsightChoice(new Player.PlayerSide[] { Player.PlayerSide.Prosecution, Player.PlayerSide.Defense },
                                game, choosingPlayer, choiceHandler, out boardChoices.MoIInfo);
                        }

                        return boardChoices;
                    },
                    (Game game, Player choosingPlayer, BoardChoices choices) =>
                    {
                        int modValue = calcModValueBasedOnSide(2, choosingPlayer);
                        choices.SelectedObjs.Keys.Cast<AspectTrack>().ToList().ForEach(t => t.AddToValue(modValue));
                        handleMomentOfInsight(game, choosingPlayer, choices);
                    }));
        }

        protected override void addSummationEventsAndChoices()
        {
            SummationEvents.Add(
                new CardEffectPair(
                    doNothingChoice,
                    (Game game, Player choosingPlayer, BoardChoices choices) =>
                    {
                        int modVal = calcModValueBasedOnSide(2, choosingPlayer);
                        int oppModVal = -calcModValueBasedOnSide(1, choosingPlayer);

                        List<AspectTrack> ats = findAspectTracksWithProp(game, Property.GovWorker, Property.Merchant, Property.Farmer);

                        ats.ForEach(t => t.AddToValue(t.Properties.Contains(Property.Farmer) ? oppModVal : modVal));
                    }));
        }
    }*/
}
