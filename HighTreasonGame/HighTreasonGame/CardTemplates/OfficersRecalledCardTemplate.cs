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
            : base("Officers Recalled", 3, Player.PlayerSide.Prosecution)
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
                    (Game game, Player choosingPlayer, BoardChoices choices) =>
                    {
                        game.Board.GetInsanityTrack().AddToValue(-1);
                    },
                    (Game game, Player choosingPlayer) =>
                    {
                        return game.OfficersRecalledPlayable;
                    }));

            TrialEvents.Add(
                new CardEffectPair(
                    doNothingChoice,
                    (Game game, Player choosingPlayer, BoardChoices choices) =>
                    {
                        int modVal = calcModValueBasedOnSide(2, choosingPlayer);

                        findAspectTracksWithProp(game, Property.Catholic)[0].AddToValue(modVal);
                    },
                    (Game game, Player choosingPlayer) =>
                    {
                        return game.OfficersRecalledPlayable;
                    }));
        }

        protected override void addSummationEventsAndChoices()
        {
            SummationEvents.Add(
                new CardEffectPair(
                    doNothingChoice,
                    (Game game, Player choosingPlayer, BoardChoices choices) =>
                    {
                        game.Board.GetInsanityTrack().AddToValue(-1);
                    },
                    (Game game, Player choosingPlayer) =>
                    {
                        return game.OfficersRecalledPlayable;
                    }));
        }
    }
}
