using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame.CardTemplates
{
    public class DoctorJamesCardTemplate : CardTemplate
    {
        public DoctorJamesCardTemplate()
            : base("Doctor James Wallace", 2)
        { }

        protected override void addSelectionEventsAndChoices()
        {
            SelectionEventChoices.Add(genRevealOrPeakCardChoice(new HashSet<Property>() { Property.Occupation }, 3, true));
            SelectionEvents.Add(revealAllAspects);

            SelectionEventChoices.Add(genRevealOrPeakCardChoice(new HashSet<Property>() { Property.Religion }, 2, true));
            SelectionEvents.Add(revealAllAspects);
        }

        protected override void addTrialEventsAndChoices()
        {
            TrialEventChoices.Add(
                (Game game, ChoiceHandler choiceHandler) =>
                {
                    BoardChoices choices = new BoardChoices();

                    choices.NotCancelled = handleMomentOfInsightChoice(new List<Player.PlayerSide>() { Player.PlayerSide.Prosecution }, game, choiceHandler, out choices.MoIInfo);

                    return choices;
                });

            TrialEvents.Add(
                (Game game, BoardChoices choices) =>
                {
                    game.GetGuiltTrack().AddToValue(-1);
                    handleMomentOfInsight(game, choices);
                });
        }

        protected override void addSummationEventsAndChoices()
        {
            SummationEventChoices.Add(doNothingChoice);

            SummationEvents.Add(
                (Game game, BoardChoices choices) =>
                {
                    List<BoardObject> options = game.FindBO(
                        (Type t) =>
                        {
                            return (t == typeof(AspectTrack));
                        },
                        (BoardObject htgo) =>
                        {
                            return (htgo.Properties.Contains(Property.Track)
                            && htgo.Properties.Contains(Property.Aspect)
                            && (htgo.Properties.Contains(Property.English) || htgo.Properties.Contains(Property.Protestant)));
                        });

                    options.Cast<AspectTrack>().ToList().ForEach(t => t.AddToValue(2));
                });
        }
    }
}
