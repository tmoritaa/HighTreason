using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame
{
    [CardTemplateAttribute]
    public class CharlesNolinCardTemplate: CardTemplate
    {
        public CharlesNolinCardTemplate()
            : base("Charles Nolin", 3, Player.PlayerSide.Prosecution)
        { }

        protected override void addSelectionEventsAndChoices()
        {
            SelectionEvents.Add(
                new CardEffectPair(
                    genRevealOrPeakCardChoice(new HashSet<Property>(), 3, true, this.CardInfo.JurySelectionInfos[0].Description),
                    revealAllAspects));
        }

        protected override void addTrialEventsAndChoices()
        {
            TrialEvents.Add(
                new CardEffectPair(
                    doNothingChoice,
                    (Game game, Player choosingPlayer, BoardChoices boardChoices) =>
                    {
                        game.Board.GetGuiltTrack().AddToValue(1);
                        findAspectTracksWithProp(game, Property.English, Property.Occupation).ForEach(t => t.AddToValue(t.Properties.Contains(Property.English) ? 2 : 1));
                    }));

            TrialEvents.Add(
                new CardEffectPair(
                    doNothingChoice,
                    (Game game, Player choosingPlayer, BoardChoices boardChoices) =>
                    {
                        game.Board.GetGuiltTrack().AddToValue(1);

                        int modVal = calcModValueBasedOnSide(2, choosingPlayer);

                        findAspectTracksWithProp(game, Property.Religion).ForEach(t => t.AddToValue(modVal));
                    }));
        }

        protected override void addSummationEventsAndChoices()
        {
            SummationEvents.Add(
                new CardEffectPair(
                    doNothingChoice,
                    (Game game, Player choosingPlayer, BoardChoices boardChoices) =>
                    {
                        findAspectTracksWithProp(game, Property.English, Property.Occupation).ForEach(t => t.AddToValue(t.Properties.Contains(Property.English) ? 3 : 1));
                    }));
        }
    }
}
