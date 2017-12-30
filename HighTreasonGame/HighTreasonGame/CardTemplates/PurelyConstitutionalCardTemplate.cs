using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json.Linq;

namespace HighTreasonGame
{
    [CardTemplateAttribute]
    public class PurelyConstitutionalCardTemplate : CardTemplate
    {
        public PurelyConstitutionalCardTemplate()
            : base("\"A Purely Constitutional Movement\"", 3, Player.PlayerSide.Defense)
        {}

        protected override void addSelectionEventsAndChoices()
        {
            SelectionEvents.Add(new CardEffectPair(
                genRevealOrPeakCardChoice(new HashSet<Property>(), 3, true, this.CardInfo.JurySelectionInfos[0].Description,
                    null,
                    (List<BoardObject> remainingChoices, Dictionary<BoardObject, int> selected) =>
                    {
                        List<BoardObject> newChoices = new List<BoardObject>(remainingChoices);
                        foreach (BoardObject obj in selected.Keys)
                        {
                            if (obj.Properties.Contains(Property.Religion))
                            {
                                newChoices = newChoices.Where(c => !c.Properties.Contains(Property.Religion)).ToList();
                            }

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
                    },
                    LimitNumAspectFilterComb(1)),
                revealAllAspects));
        }

        protected override void addTrialEventsAndChoices()
        {
            TrialEvents.Add(
                new CardEffectPair(
                    (Game game, Player choosingPlayer) =>
                    {
                        return handleMomentOfInsightChoice(new Player.PlayerSide[] { Player.PlayerSide.Defense }, game, choosingPlayer);
                    },
                    (Game game, Player choosingPlayer, BoardChoices choices) => 
                    {
                        game.Board.GetInsanityTrack().AddToValue(1);
                        handleMomentOfInsight(game, choosingPlayer, choices);
                    }));

            TrialEvents.Add(
                new CardEffectPair(
                    doNothingChoice,
                    (Game game, Player choosingPlayer, BoardChoices choices) => 
                    {
                        findAspectTracksWithProp(game, Property.Farmer, Property.French).ForEach(t => t.AddToValue(-2));
                    }));
        }

        protected override void addSummationEventsAndChoices()
        {
            SummationEvents.Add(
                new CardEffectPair(
                    doNothingChoice,
                    (Game game, Player choosingPlayer, BoardChoices choices) => 
                    {
                        game.Board.GetInsanityTrack().AddToValue(1);
                    }));
        }
    }
}
