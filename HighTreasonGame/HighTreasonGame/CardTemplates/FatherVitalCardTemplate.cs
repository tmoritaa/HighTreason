using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json.Linq;

namespace HighTreasonGame.CardTemplates
{
    public class FatherVitalCardTemplate : CardTemplate
    {
        public FatherVitalCardTemplate(JObject json)
            : base("Father Vital Fourmond", 2, json)
        { }

        protected override void addSelectionEventsAndChoices()
        {
            SelectionEvents.Add(
                new CardEffectPair(
                    genRevealOrPeakCardChoice(new HashSet<Property>(), 2, true, this.CardInfo.JurySelectionPairs[0].Description),
                    revealAllAspects));

            SelectionEvents.Add(
                new CardEffectPair(
                    genRevealOrPeakCardChoice(new HashSet<Property>() { Property.Religion }, 1, false, this.CardInfo.JurySelectionPairs[1].Description),
                    peekAllAspects));
        }

        protected override void addTrialEventsAndChoices()
        {            
            TrialEvents.Add(
                new CardEffectPair(
                    doNothingChoice,
                    (Game game, BoardChoices choices) =>
                    {
                        game.GetInsanityTrack().AddToValue(1);
                    }));

            TrialEvents.Add(
                new CardEffectPair(
                    genRevealOrPeakCardChoice(new HashSet<Property>() { Property.Religion }, 3, false, this.CardInfo.TrialInChiefPairs[1].Description),
                    peekAllAspects));
        }

        protected override void addSummationEventsAndChoices()
        {
            SummationEvents.Add(
                new CardEffectPair(
                    doNothingChoice,
                    (Game game, BoardChoices choices) =>
                    {
                        game.GetInsanityTrack().AddToValue(1);
                    }));
        }
    }
}
