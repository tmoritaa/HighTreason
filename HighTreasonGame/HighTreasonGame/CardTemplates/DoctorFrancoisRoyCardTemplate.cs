using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame
{
    [CardTemplateAttribute]
    public class DoctorFrancoisRoyCardTemplate : CardTemplate
    {
        public DoctorFrancoisRoyCardTemplate()
            : base("Doctor Francois Roy", 3, Player.PlayerSide.Defense)
        { }

        protected override void addSelectionEventsAndChoices()
        {
            SelectionEvents.Add(
                new CardEffectPair(
                    genRevealOrPeakCardChoice(new HashSet<Property>(), 2, false, this.CardInfo.JurySelectionInfos[0].Description),
                    peekAllAspects));
        }

        protected override void addTrialEventsAndChoices()
        {
            TrialEvents.Add(
                new CardEffectPair(
                    doNothingChoice,
                    (Game game, Player choosingPlayer, BoardChoices boardChoices) =>
                    {
                        game.Board.GetInsanityTrack().AddToValue(1);
                        findAspectTracksWithProp(game, Property.French, Property.Protestant, Property.Catholic).ForEach(t => t.AddToValue(-2));
                    }));
        }

        protected override void addSummationEventsAndChoices()
        {
            SummationEvents.Add(
                new CardEffectPair(
                    doNothingChoice,
                    (Game game, Player choosingPlayer, BoardChoices boardChoices) =>
                    {
                        game.Board.GetInsanityTrack().AddToValue(1);
                    }));

            SummationEvents.Add(
                new CardEffectPair(
                    (Game game, Player choosingPlayer) =>
                    {
                        List<BoardObject> options = game.FindBO(
                            (BoardObject htgo) =>
                            {
                                return (htgo.Properties.Contains(Property.Track)
                                && htgo.Properties.Contains(Property.Aspect)
                                && !htgo.Properties.Contains(Property.English)
                                && ((Track)htgo).CanModify(2));
                            });

                        return new HTAction(choosingPlayer.ChoiceHandler).InitForChooseBOs(
                            (List<BoardObject> choices) =>
                            {
                                return HTUtility.FindAllCombOfBoardObjs(choices, 3);
                            },
                            options,
                            (Dictionary<BoardObject, int> selected) => { return true; },
                            (List<BoardObject> remainingChoices, Dictionary<BoardObject, int> selected) =>
                            {
                                return remainingChoices.Where(obj => !selected.ContainsKey(obj)).ToList();
                            },
                            (Dictionary<BoardObject, int> selected) => { return selected.Keys.Count == 3; },
                            game,
                            choosingPlayer,
                            this.CardInfo.SummationInfos[1].Description);
                    },
                    (Game game, Player choosingPlayer, BoardChoices boardChoices) =>
                    {
                        boardChoices.SelectedObjs.Keys.Cast<AspectTrack>().ToList().ForEach(t => t.AddToValue(-2));
                    }));
        }
    }
}
