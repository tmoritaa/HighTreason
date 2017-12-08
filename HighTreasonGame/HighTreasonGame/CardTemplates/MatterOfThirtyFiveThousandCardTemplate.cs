using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame
{
    [CardTemplateAttribute]
    public class MatterOfThirtyFiveThousand : CardTemplate
    {
        public MatterOfThirtyFiveThousand()
            : base("A Matter of $35000", 2, Player.PlayerSide.Prosecution)
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
                        game.Board.GetInsanityTrack().AddToValue(-1);
                        findAspectTracksWithProp(game, Property.French)[0].AddToValue(2);
                    }));
        }

        protected override void addSummationEventsAndChoices()
        {
            SummationEvents.Add(
                new CardEffectPair(
                    doNothingChoice,
                    (Game game, Player choosingPlayer, BoardChoices boardChoices) =>
                    {
                        game.Board.GetInsanityTrack().AddToValue(-1);
                        findAspectTracksWithProp(game, Property.French, Property.Catholic, Property.Farmer).ForEach(t => t.AddToValue(1));
                    }));
        }
    }
}
