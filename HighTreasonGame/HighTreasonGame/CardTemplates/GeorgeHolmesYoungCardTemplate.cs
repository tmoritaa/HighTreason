using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json.Linq;

namespace HighTreasonGame
{
    [CardTemplateAttribute]
    public class GeorgeHolmesYoungCardTemplate : CardTemplate
    {
        public GeorgeHolmesYoungCardTemplate()
            : base("George Holmes Young", 2, Player.PlayerSide.Prosecution)
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
                    genAspectTrackForModCardChoice(new HashSet<Property>(), 1, 1, false, this.CardInfo.TrialInChiefInfos[0].Description),
                    (Game game, Player choosingPlayer, BoardChoices choices) =>
                    {
                        raiseGuiltAndOneAspectEffect(game, choosingPlayer, choices);
                        game.OfficersRecalledPlayable = true;
                    }));
        }

        protected override void addSummationEventsAndChoices()
        {
            SummationEvents.Add(
                new CardEffectPair(
                    doNothingChoice,
                    (Game game, Player choosingPlayer, BoardChoices choices) =>
                    {
                        findAspectTracksWithProp(game, Property.Occupation).ForEach(t => t.AddToValue(1));

                        game.OfficersRecalledPlayable = true;
                    }));
        }
    }
}
