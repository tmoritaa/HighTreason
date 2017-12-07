using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame
{
    [CardTemplateAttribute]
    public class OfficersRecalledCardTemplate : CardTemplate
    {
        public OfficersRecalledCardTemplate()
            : base("Officers Recalled", 3)
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
                    doNothingChoice,
                    (Game game, BoardChoices choices) =>
                    {
                        game.GetInsanityTrack().AddToValue(-1);
                    },
                    (Game game) =>
                    {
                        return game.OfficersRecalledPlayable;
                    }));

            TrialEvents.Add(
                new CardEffectPair(
                    doNothingChoice,
                    (Game game, BoardChoices choices) =>
                    {
                        int modVal = calcModValueBasedOnSide(2, game);

                        findAspectTracksWithProp(game, Property.Catholic)[0].AddToValue(modVal);
                    },
                    (Game game) =>
                    {
                        return game.OfficersRecalledPlayable;
                    }));
        }

        protected override void addSummationEventsAndChoices()
        {
            SummationEvents.Add(
                new CardEffectPair(
                    doNothingChoice,
                    (Game game, BoardChoices choices) =>
                    {
                        game.GetInsanityTrack().AddToValue(-1);
                    },
                    (Game game) =>
                    {
                        return game.OfficersRecalledPlayable;
                    }));
        }
    }
}
