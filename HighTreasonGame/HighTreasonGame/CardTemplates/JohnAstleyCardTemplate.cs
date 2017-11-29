using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json.Linq;

namespace HighTreasonGame.CardTemplates
{
    public class JohnAstleyCardTemplate : CardTemplate
    {
        public JohnAstleyCardTemplate(JObject json) 
            : base("John W. Astley", 2, json)
        {}

        protected override void addSelectionEventsAndChoices()
        {
            SelectionEventChoices.Add(genRevealOrPeakCardChoice(new HashSet<Property>() { Property.Occupation }, 3, true));
            SelectionEvents.Add(revealAllAspects);

            SelectionEventChoices.Add(genRevealOrPeakCardChoice(new HashSet<Property>() { Property.Religion }, 2, true));
            SelectionEvents.Add(revealAllAspects);
        }

        protected override void addTrialEventsAndChoices()
        {
            TrialEventChoices.Add(genAspectTrackForModCardChoice(new HashSet<Property>(), 1, 1, false));
            TrialEvents.Add(raiseGuiltAndOneAspectEffect);
        }

        protected override void addSummationEventsAndChoices()
        {
            SummationEventChoices.Add(
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
                        out boardChoices);

                    return boardChoices;
                });

            SummationEvents.Add(
                (Game game, BoardChoices choices) => 
                {
                    choices.SelectedObjs.Keys.Cast<AspectTrack>().ToList().ForEach(t => t.AddToValue(1));
                });
        }
    }
}
