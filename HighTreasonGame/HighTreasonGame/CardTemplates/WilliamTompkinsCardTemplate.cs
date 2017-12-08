using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json.Linq;

namespace HighTreasonGame.CardTemplates
{
    [CardTemplateAttribute]
    public class WilliamTompkinsCardTemplate : CardTemplate
    {
        public WilliamTompkinsCardTemplate()
            : base("William Tompkins", 3, Player.PlayerSide.Prosecution)
        { }

        protected override void addSelectionEventsAndChoices()
        {
            SelectionEvents.Add(
                new CardEffectPair(
                    genRevealOrPeakCardChoice(new HashSet<Property> { Property.Occupation }, 3, true, this.CardInfo.JurySelectionInfos[0].Description),
                    revealAllAspects));

            SelectionEvents.Add(
                new CardEffectPair(
                    genRevealOrPeakCardChoice(new HashSet<Property> { Property.Language }, 2, true, this.CardInfo.JurySelectionInfos[1].Description),
                    revealAllAspects));

            SelectionEvents.Add(
                new CardEffectPair(
                    genRevealOrPeakCardChoice(new HashSet<Property>() { Property.Religion }, 1, true, this.CardInfo.JurySelectionInfos[2].Description),
                    revealAllAspects));
        }

        protected override void addTrialEventsAndChoices()
        {
            TrialEvents.Add(
                new CardEffectPair(
                    (Game game, Player choosingPlayer, ChoiceHandler choiceHandler) =>
                    {
                        int modValue = calcModValueBasedOnSide(2, choosingPlayer);

                        List<BoardObject> options = game.FindBO(
                            (BoardObject htgo) =>
                            {
                                return (htgo.Properties.Contains(Property.Track)
                                && htgo.Properties.Contains(Property.Aspect)
                                && htgo.Properties.Contains(Property.Occupation)
                                && ((Track)htgo).CanModify(modValue));
                            });

                        BoardChoices boardChoices;
                        choiceHandler.ChooseBoardObjects(
                            options,
                            (Dictionary<BoardObject, int> selected) => { return true; },
                            (List<BoardObject> remainingChoices, Dictionary<BoardObject, int> selected) =>
                            {
                                return remainingChoices.Where(obj => !selected.ContainsKey(obj)).ToList();
                            },
                            (Dictionary<BoardObject, int> selected) => { return selected.Keys.Count == 1; },
                            game,
                            choosingPlayer,
                            this.CardInfo.TrialInChiefInfos[0].Description,
                            out boardChoices);

                        if (boardChoices.NotCancelled)
                        {
                            boardChoices.NotCancelled = handleMomentOfInsightChoice(new Player.PlayerSide[] { Player.PlayerSide.Prosecution, Player.PlayerSide.Defense },
                                game, choosingPlayer, choiceHandler, out boardChoices.MoIInfo);
                        }

                        return boardChoices;
                    },
                    (Game game, Player choosingPlayer, BoardChoices choices) =>
                    {
                        int modValue = calcModValueBasedOnSide(2, choosingPlayer);
                        choices.SelectedObjs.Keys.Cast<AspectTrack>().ToList().ForEach(t => t.AddToValue(modValue));
                        handleMomentOfInsight(game, choosingPlayer, choices);
                    }));
        }

        protected override void addSummationEventsAndChoices()
        {
            SummationEvents.Add(
                new CardEffectPair(
                    doNothingChoice,
                    (Game game, Player choosingPlayer, BoardChoices choices) =>
                    {
                        int modVal = calcModValueBasedOnSide(2, choosingPlayer);
                        int oppModVal = -calcModValueBasedOnSide(1, choosingPlayer);

                        List<AspectTrack> ats = findAspectTracksWithProp(game, Property.GovWorker, Property.Merchant, Property.Farmer);

                        ats.ForEach(t => t.AddToValue(t.Properties.Contains(Property.Farmer) ? oppModVal : modVal));
                    }));
        }
    }
}
