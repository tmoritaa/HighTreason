using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame
{
    [CardTemplateAttribute]
    public class ExovedeCardTemplate : CardTemplate
    {
        public ExovedeCardTemplate()
            : base("Exovede", 3, Player.PlayerSide.Defense)
        { }

        protected override void addSelectionEventsAndChoices()
        {
            SelectionEvents.Add(
                new CardEffectPair(
                    genRevealOrPeakCardChoice(new HashSet<Property>() { Property.Occupation, Property.Language }, 2, true, this.CardInfo.JurySelectionInfos[0].Description, 
                        null,
                        (List<BoardObject> remainingChoices, Dictionary<BoardObject, int> selected) =>
                        {
                            List<BoardObject> newChoices = new List<BoardObject>(remainingChoices);
                            foreach (BoardObject obj in selected.Keys)
                            {
                                if (obj.Properties.Contains(Property.Occupation))
                                {
                                    newChoices = newChoices.Where(c => !c.Properties.Contains(Property.Occupation)).ToList();
                                }

                                if (obj.Properties.Contains(Property.Language))
                                {
                                    newChoices = newChoices.Where(c => !c.Properties.Contains(Property.Language)).ToList();
                                }
                            }

                            return newChoices;
                        }),
                    revealAllAspects));

            SelectionEvents.Add(
                new CardEffectPair(
                    genRevealOrPeakCardChoice(new HashSet<Property>() { Property.Religion }, 2, false, this.CardInfo.JurySelectionInfos[1].Description),
                    peekAllAspects));
        }

        protected override void addTrialEventsAndChoices()
        {
            TrialEvents.Add(
                new CardEffectPair(
                    doNothingChoice,
                    (Game game, Player choosingPlayer, BoardChoices boardChoices) =>
                    {
                        int protModVal = calcModValueBasedOnSide(2, choosingPlayer);
                        int cathModVal = calcModValueBasedOnSide(1, choosingPlayer);
                        findAspectTracksWithProp(game, Property.Protestant, Property.Catholic).ForEach(t => t.AddToValue(t.Properties.Contains(Property.Protestant) ? protModVal : cathModVal));
                    }));
        }

        protected override void addSummationEventsAndChoices()
        {
            SummationEvents.Add(
                new CardEffectPair(
                    doNothingChoice,
                    (Game game, Player choosingPlayer, BoardChoices boardChoices) =>
                    {
                        int modVal = calcModValueBasedOnSide(1, choosingPlayer);
                        findAspectTracksWithProp(game, Property.Protestant, Property.Catholic).ForEach(t => t.AddToValue(modVal));
                    }));
        }
    }
}
