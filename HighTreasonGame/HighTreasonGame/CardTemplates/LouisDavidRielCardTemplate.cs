using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame
{
    [CardTemplateAttribute]
    public class LouisDavidRielCardTemplate : CardTemplate
    {
        public LouisDavidRielCardTemplate()
            : base("Louis David Riel", 5, Player.PlayerSide.Defense)
        { }

        protected override void addSelectionEventsAndChoices()
        {
            SelectionEvents.Add(
                new CardEffectPair(
                    genRevealOrPeakCardChoice(
                        new HashSet<Property>(), 5, true, this.CardInfo.JurySelectionInfos[0].Description,
                        null,
                        (List<BoardObject> choices, Dictionary<BoardObject, int> selected) =>
                        {
                            List<BoardObject> newChoices = new List<BoardObject>(choices);

                            Property[] props = new Property[] { Property.Religion, Property.Language, Property.Occupation };
                            int[] numProps = new int[] { 0, 0, 0 };
                            
                            foreach (var pairs in selected)
                            {
                                newChoices.Remove(pairs.Key);
                                
                                for (int i = 0; i < 3; ++i)
                                {
                                    if (pairs.Key.Properties.Contains(props[i]))
                                    {
                                        numProps[i] += 1;
                                    }
                                }
                            }

                            if (numProps[0] >= 3)
                            {
                                newChoices = newChoices.Where(bo => !bo.Properties.Contains(Property.Religion)).ToList();
                            }

                            if (numProps[1] >= 3)
                            {
                                newChoices = newChoices.Where(bo => !bo.Properties.Contains(Property.Language)).ToList();
                            }

                            if (numProps[2] >= 3)
                            {
                                newChoices = newChoices.Where(bo => !bo.Properties.Contains(Property.Occupation)).ToList();
                            }

                            return newChoices;
                        }),
                    revealAllAspects));
        }

        protected override void addTrialEventsAndChoices()
        {
            TrialEvents.Add(
                new CardEffectPair(
                    (Game game, Player choosingPlayer) =>
                    {
                        List<BoardObject> bos = findAspectTracksWithProp(game).Cast<BoardObject>().ToList();

                        return new HTAction(choosingPlayer.ChoiceHandler).InitForChooseBOs(
                            bos,
                            (Dictionary<BoardObject, int> selected) => { return true; },
                            (List<BoardObject> choices, Dictionary<BoardObject, int> selected) =>
                            {
                                List<BoardObject> newChoices = new List<BoardObject>(choices);
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
                            (Dictionary<BoardObject, int> selected) =>
                            {
                                return selected.Count == 3;
                            },
                            game,
                            choosingPlayer,
                            this.CardInfo.TrialInChiefInfos[0].Description);
                    },
                    (Game game, Player choosingPlayer, BoardChoices boardChoices) =>
                    {
                        game.Board.GetInsanityTrack().AddToValue(-calcModValueBasedOnSide(1, choosingPlayer));

                        boardChoices.SelectedObjs.Keys.Cast<AspectTrack>().ToList().ForEach(t => t.AddToValue(calcModValueBasedOnSide(1, choosingPlayer)));
                    }));
        }

        protected override void addSummationEventsAndChoices()
        {
            SummationEvents.Add(
                new CardEffectPair(
                    doNothingChoice,
                    (Game game, Player choosingPlayer, BoardChoices boardChoices) =>
                    {
                        game.Board.GetInsanityTrack().AddToValue(-calcModValueBasedOnSide(1, choosingPlayer));
                    }));

            SummationEvents.Add(
                new CardEffectPair(
                    genAspectTrackForModCardChoice(new HashSet<Property>(), 2, 2, true, this.CardInfo.SummationInfos[1].Description),
                    (Game game, Player choosingPlayer, BoardChoices boardChoices) =>
                    {
                        boardChoices.SelectedObjs.Keys.Cast<AspectTrack>().ToList().ForEach(t => t.AddToValue(calcModValueBasedOnSide(2, choosingPlayer)));
                    }));
        }
    }
}
