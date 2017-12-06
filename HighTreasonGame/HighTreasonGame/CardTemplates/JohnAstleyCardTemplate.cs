using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json.Linq;

namespace HighTreasonGame.CardTemplates
{
    [CardTemplateAttribute]
    public class JohnAstleyCardTemplate : CardTemplate
    {
        public JohnAstleyCardTemplate() 
            : base("John W. Astley", 2)
        {}

        protected override void addSelectionEventsAndChoices()
        {
            SelectionEvents.Add(
                new CardEffectPair(
                    genRevealOrPeakCardChoice(new HashSet<Property>() { Property.Occupation }, 3, true, this.CardInfo.JurySelectionInfos[0].Description),
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
                    (Game game, ChoiceHandler choiceHandler) =>
                    {
                        List<BoardObject> options = game.FindBO(
                            (BoardObject htgo) =>
                            {
                                return (htgo.Properties.Contains(Property.Track)
                                && htgo.Properties.Contains(Property.Aspect)
                                && !htgo.Properties.Contains(Property.French)
                                && ((Track)htgo).CanModify(1));
                            });

                        BoardChoices boardChoices;
                        choiceHandler.ChooseBoardObjects(
                            options,
                            (Dictionary<BoardObject, int> selected) => { return true; },
                            (List<BoardObject> remainingChoices, Dictionary<BoardObject, int> selected) =>
                            {
                                return remainingChoices.Where(obj => !selected.ContainsKey(obj)).ToList();
                            },
                            (Dictionary<BoardObject, int> selected) => { return selected.Keys.Count == 3; },
                            game,
                            this.CardInfo.SummationInfos[0].Description,
                            out boardChoices);

                        return boardChoices;
                    },
                    (Game game, BoardChoices choices) => 
                    {
                        choices.SelectedObjs.Keys.Cast<AspectTrack>().ToList().ForEach(t => t.AddToValue(1));
                    }));
        }
    }
}
