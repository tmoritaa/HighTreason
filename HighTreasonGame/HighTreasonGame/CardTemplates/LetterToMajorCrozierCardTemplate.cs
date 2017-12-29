using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame
{
    [CardTemplateAttribute]
    public class LetterToMajorCrozierCardTemplate : CardTemplate
    {
        public LetterToMajorCrozierCardTemplate() 
            : base("Letter to Major Crozier", 2, Player.PlayerSide.Prosecution)
        { }

        protected override void addSelectionEventsAndChoices()
        {
            SelectionEvents.Add(
                new CardEffectPair(
                    genRevealOrPeakCardChoice(new HashSet<Property>() { Property.Occupation }, 4, true, this.CardInfo.JurySelectionInfos[0].Description),
                    revealAllAspects));

            SelectionEvents.Add(
                new CardEffectPair(
                    genRevealOrPeakCardChoice(new HashSet<Property>() { Property.Religion }, 2, true, this.CardInfo.JurySelectionInfos[1].Description),
                    revealAllAspects));
        }

        protected override void addTrialEventsAndChoices()
        {
            TrialEvents.Add(
                new CardEffectPair(
                    genAspectTrackForModCardChoice(new HashSet<Property>(), 1, 1, false, this.CardInfo.TrialInChiefInfos[0].Description),
                    raiseGuiltAndOneAspectEffect));
        }

        protected override void addSummationEventsAndChoices()
        {
            SummationEvents.Add(
                new CardEffectPair(
                    (Game game, Player choosingPlayer) =>
                    {
                        List<BoardObject> options = game.FindBO(
                            (BoardObject htgo) =>
                            {
                                return (htgo.Properties.Contains(Property.Track)
                                && htgo.Properties.Contains(Property.Aspect)
                                && ((Track)htgo).CanModify(1));
                            });

                        return new HTAction(
                            ChoiceHandler.ChoiceType.BoardObjects,
                            choosingPlayer.ChoiceHandler,
                            options,
                            (Func<Dictionary<BoardObject, int>, bool>)((Dictionary<BoardObject, int> selected) => { return true; }),
                            (Func<List<BoardObject>, Dictionary<BoardObject, int>, List<BoardObject>>)
                                ((List<BoardObject> remainingChoices, Dictionary<BoardObject, int> selected) =>
                                {
                                    return remainingChoices.Where(obj => !selected.ContainsKey(obj)).ToList();
                                }),
                            (Func<Dictionary<BoardObject, int>, bool>)((Dictionary<BoardObject, int> selected) => { return selected.Keys.Count == 4; }),
                            game,
                            choosingPlayer,
                            this.CardInfo.SummationInfos[0].Description);
                    },
                    (Game game, Player choosingPlayer, BoardChoices choices) =>
                    {
                        choices.SelectedObjs.Keys.Cast<AspectTrack>().ToList().ForEach(t => t.AddToValue(1));
                    }));
        }
    }
}
