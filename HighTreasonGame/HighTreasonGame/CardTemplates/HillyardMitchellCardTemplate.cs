using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json.Linq;

namespace HighTreasonGame
{
    /*
    [CardTemplateAttribute]
    public class HillyardMitchellCardTemplate : CardTemplate
    {
        public HillyardMitchellCardTemplate()
            : base("Hillyard Mitchell", 2, Player.PlayerSide.Prosecution)
        { }

        protected override void addSelectionEventsAndChoices()
        {
            SelectionEvents.Add(
                new CardEffectPair(
                    genRevealOrPeakCardChoice(new HashSet<Property> { Property.Occupation }, 3, true, this.CardInfo.JurySelectionInfos[0].Description),
                    revealAllAspects));

            SelectionEvents.Add(
                new CardEffectPair(
                    genRevealOrPeakCardChoice(new HashSet<Property> { Property.Language }, 3, true, this.CardInfo.JurySelectionInfos[1].Description),
                    revealAllAspects));
        }

        protected override void addTrialEventsAndChoices()
        {
            TrialEvents.Add(
                new CardEffectPair(
                    (Game game, Player choosingPlayer, ChoiceHandler choiceHandler) =>
                    {
                        CardChoice peekFunc = genRevealOrPeakCardChoice(new HashSet<Property>(), 2, false, this.CardInfo.TrialInChiefInfos[0].Description);
                        BoardChoices boardChoices = peekFunc(game, choosingPlayer, choiceHandler);

                        if (boardChoices.NotCancelled)
                        {
                            boardChoices.NotCancelled = handleMomentOfInsightChoice(new Player.PlayerSide[] { Player.PlayerSide.Prosecution, Player.PlayerSide.Defense },
                                game, choosingPlayer, choiceHandler, out boardChoices.MoIInfo);
                        }

                        return boardChoices;
                    },
                    (Game game, Player choosingPlayer, BoardChoices choices) =>
                    {
                        peekAllAspects(game, choosingPlayer, choices);
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
                        findAspectTracksWithProp(game, Property.Merchant, Property.Farmer).ForEach(t => t.AddToValue(2));
                    }));
        }
    }*/
}
