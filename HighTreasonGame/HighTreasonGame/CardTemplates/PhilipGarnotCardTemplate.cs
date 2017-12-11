using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame
{
    [CardTemplateAttribute]
    public class PhilipGarnotCardTemplate : CardTemplate
    {
        public PhilipGarnotCardTemplate()
            : base("Philip Garnot", 3, Player.PlayerSide.Defense)
        { }

        protected override void addSelectionEventsAndChoices()
        {
            SelectionEvents.Add(
                new CardEffectPair(
                    genRevealOrPeakCardChoice(new HashSet<Property>() { Property.Language }, 3, true, this.CardInfo.JurySelectionInfos[0].Description),
                    revealAllAspects));

            SelectionEvents.Add(
                new CardEffectPair(
                    genRevealOrPeakCardChoice(new HashSet<Property>() { Property.Occupation }, 2, true, this.CardInfo.JurySelectionInfos[1].Description),
                    revealAllAspects));
        }

        protected override void addTrialEventsAndChoices()
        {
            TrialEvents.Add(
                new CardEffectPair(
                    doNothingChoice,
                    (Game game, Player choosingPlayer, BoardChoices boardChoices) =>
                    {
                        game.Board.GetInsanityTrack().AddToValue(1);
                        findAspectTracksWithProp(game, Property.French, Property.Catholic).ForEach(t => t.AddToValue(-1));
                    }));
        }

        protected override void addSummationEventsAndChoices()
        {
            SummationEvents.Add(
                new CardEffectPair(
                    doNothingChoice,
                    (Game game, Player choosingPlayer, BoardChoices boardChoices) =>
                    {
                        findAspectTracksWithProp(game, Property.French, Property.Catholic, Property.Farmer).ForEach(t => t.AddToValue(t.Properties.Contains(Property.Farmer) ? -1 : -2));
                    }));
        }
    }
}
