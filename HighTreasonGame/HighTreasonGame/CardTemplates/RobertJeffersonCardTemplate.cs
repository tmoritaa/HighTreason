using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame
{
    [CardTemplateAttribute]
    public class RobertJeffersonCardTemplate : CardTemplate
    {
        public RobertJeffersonCardTemplate() 
            : base("Robert Jefferson", 2, Player.PlayerSide.Prosecution)
        { }

        protected override void addSelectionEventsAndChoices()
        {
            SelectionEvents.Add(
                new CardEffectPair(
                    genRevealOrPeakCardChoice(new HashSet<Property>() { Property.Language }, 2, true, this.CardInfo.JurySelectionInfos[0].Description),
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
                    (Game game, Player choosingPlayer, BoardChoices choices) => 
                    {
                        List<AspectTrack> ts = findAspectTracksWithProp(game, new Property[] { Property.Farmer, Property.Merchant });
                        ts.ForEach(t => t.AddToValue(t.Properties.Contains(Property.Farmer) ? 2 : 1));
                    }));
        }

        protected override void addSummationEventsAndChoices()
        {
            SummationEvents.Add(
                new CardEffectPair(
                    doNothingChoice,
                    (Game game, Player choosingPlayer, BoardChoices choices) =>
                    {
                        findAspectTracksWithProp(game, new Property[] { Property.Merchant })[0].AddToValue(2);
                    }));
        }
    }
}
