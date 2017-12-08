using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json.Linq;

namespace HighTreasonGame.CardTemplates
{
    [CardTemplateAttribute]
    public class FatherVitalCardTemplate : CardTemplate
    {
        public FatherVitalCardTemplate()
            : base("Father Vital Fourmond", 2, Player.PlayerSide.Defense)
        { }

        protected override void addSelectionEventsAndChoices()
        {
            SelectionEvents.Add(
                new CardEffectPair(
                    genRevealOrPeakCardChoice(new HashSet<Property>(), 2, true, this.CardInfo.JurySelectionInfos[0].Description),
                    revealAllAspects));

            SelectionEvents.Add(
                new CardEffectPair(
                    genRevealOrPeakCardChoice(new HashSet<Property>() { Property.Religion }, 1, false, this.CardInfo.JurySelectionInfos[1].Description),
                    peekAllAspects));
        }

        protected override void addTrialEventsAndChoices()
        {            
            TrialEvents.Add(
                new CardEffectPair(
                    doNothingChoice,
                    (Game game, Player choosingPlayer, BoardChoices choices) =>
                    {
                        game.Board.GetInsanityTrack().AddToValue(1);
                    }));

            TrialEvents.Add(
                new CardEffectPair(
                    genRevealOrPeakCardChoice(new HashSet<Property>() { Property.Religion }, 3, false, this.CardInfo.TrialInChiefInfos[1].Description),
                    peekAllAspects));
        }

        protected override void addSummationEventsAndChoices()
        {
            SummationEvents.Add(
                new CardEffectPair(
                    doNothingChoice,
                    (Game game, Player choosingPlayer, BoardChoices choices) =>
                    {
                        game.Board.GetInsanityTrack().AddToValue(1);
                    }));
        }
    }
}
