using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame.CardTemplates
{
    public class HaroldRossCardTemplate : CardTemplate
    {
        public HaroldRossCardTemplate()
            : base("Harold Ross", 3)
        { }

        protected override void addSelectionEventsAndChoices()
        {
            SelectionEventChoices.Add(genRevealOrPeakCardChoice(new HashSet<Property>() { Property.Occupation }, 3, true));
            SelectionEvents.Add(revealAllAspects);

            SelectionEventChoices.Add(genRevealOrPeakCardChoice(new HashSet<Property>() { Property.Language }, 4, true));
            SelectionEvents.Add(revealAllAspects);
        }

        protected override void addTrialEventsAndChoices()
        {
            TrialEventChoices.Add(genAspectTrackForModCardChoice(new HashSet<Property>(), 1, 1, false));
            TrialEvents.Add(
                (Game game, BoardChoices choices) =>
                {
                    game.GetGuiltTrack().AddToValue(1);
                    choices.SelectedObjs.Keys.Cast<AspectTrack>().ToList().ForEach(t => t.AddToValue(1));
                });

            TrialEventChoices.Add(genAspectTrackForModCardChoice(new HashSet<Property>(), 1, 2, true));
            TrialEvents.Add(
                (Game game, BoardChoices choices) =>
                {
                    int modValue = calcModValueBasedOnSide(2, game);
                    choices.SelectedObjs.Keys.Cast<AspectTrack>().ToList().ForEach(t => t.AddToValue(modValue));
                });
        }

        protected override void addSummationEventsAndChoices()
        {
            SummationEventChoices.Add(doNothingChoice);

            SummationEvents.Add(
                (Game game, BoardChoices choices) =>
                {
                    List<BoardObject> options = game.FindBO(
                        (BoardObject htgo) =>
                        {
                            return (htgo.Properties.Contains(Property.Track)
                            && htgo.Properties.Contains(Property.Aspect)
                            && (htgo.Properties.Contains(Property.English) || htgo.Properties.Contains(Property.GovWorker)));
                        });

                    options.Cast<AspectTrack>().ToList().ForEach(t => t.AddToValue(1));
                });

            SummationEventChoices.Add(doNothingChoice);

            SummationEvents.Add(
                (Game game, BoardChoices choices) =>
                {
                    List<BoardObject> options = game.FindBO(
                        (BoardObject htgo) =>
                        {
                            return (htgo.Properties.Contains(Property.Track)
                            && htgo.Properties.Contains(Property.Aspect)
                            && (htgo.Properties.Contains(Property.Farmer) || htgo.Properties.Contains(Property.French)));
                        });

                    options.Cast<AspectTrack>().ToList().ForEach(t => t.AddToValue(-1));
                });
        }
    }
}
