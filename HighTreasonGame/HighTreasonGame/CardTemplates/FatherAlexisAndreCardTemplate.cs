using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame
{
    [CardTemplateAttribute]
    public class FatherAlexisAndreCardTemplate : CardTemplate
    {
        // NOTE: type should be neutral, but keeping prosecution since it literally doesn't matter.
        public FatherAlexisAndreCardTemplate()
            : base("Father Alexis Andre", 3, Player.PlayerSide.Prosecution)
        { }

        protected override void addSelectionEventsAndChoices()
        {
            SelectionEvents.Add(
                new CardEffectPair(
                    genRevealOrPeakCardChoice(new HashSet<Property>(), 3, true, this.CardInfo.JurySelectionInfos[0].Description),
                    revealAllAspects));

            SelectionEvents.Add(
                new CardEffectPair(
                    genRevealOrPeakCardChoice(new HashSet<Property>() { Property.Religion }, 2, false, this.CardInfo.JurySelectionInfos[0].Description),
                    peekAllAspects));
        }

        protected override void addTrialEventsAndChoices()
        {
            TrialEvents.Add(
                new CardEffectPair(
                    doNothingChoice,
                    (Game game, Player choosingPlayer, BoardChoices boardChoices) =>
                    {
                        int modVal = calcModValueBasedOnSide(2, choosingPlayer);
                        findAspectTracksWithProp(game, Property.French, Property.Catholic, Property.Farmer).ForEach(t => t.AddToValue(modVal));
                    }));
        }

        protected override void addSummationEventsAndChoices()
        {
            SummationEvents.Add(
                new CardEffectPair(
                    doNothingChoice,
                    (Game game, Player choosingPlayer, BoardChoices boardChoices) =>
                    {
                        int modVal = -calcModValueBasedOnSide(1, choosingPlayer);
                        game.Board.GetInsanityTrack().AddToValue(modVal);
                    }));
        }
    }
}
