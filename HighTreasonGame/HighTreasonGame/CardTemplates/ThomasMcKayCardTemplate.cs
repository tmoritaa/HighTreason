using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame.CardTemplates
{
    public class ThomasMcKayCardTemplate : CardTemplate
    {
        public ThomasMcKayCardTemplate()
            : base("Thomas McKay", 2)
        { }

        protected override void addSelectionEventsAndChoices()
        {
            SelectionEventChoices.Add(
                (Game game, ChoiceHandler choiceHandler) =>
                {
                    List<Jury.JuryAspect> juryAspects = new List<Jury.JuryAspect>();

                    List<HTGameObject> options = game.GetHTGOFromCondition(
                            (HTGameObject htgo) =>
                            {
                                return (htgo.Properties.Contains(Property.Jury)
                                && htgo.Properties.Contains(Property.Aspect)
                                && !((Jury.JuryAspect)htgo).IsRevealed);
                            });

                    BoardChoices choices = new BoardChoices();
                    choices.NotCancelled = choiceHandler.ChooseJuryAspects(new List<List<HTGameObject>>() { options }, new List<int>() { 2 },
                        game, out choices.JuryAspects);

                    return choices;
                });

            SelectionEvents.Add(revealAllAspects);
        }

        protected override void addTrialEventsAndChoices()
        {
            TrialEventChoices.Add(pickOneAnyAspectChoice);
            TrialEvents.Add(raiseGuiltAndOneAspectEffect);
        }

        protected override void addSummationEventsAndChoices()
        {
            SummationEventChoices.Add(doNothingChoice);

            SummationEvents.Add(
                (Game game, BoardChoices choices) =>
                {
                    List<HTGameObject> options = game.GetHTGOFromCondition(
                            (HTGameObject htgo) =>
                            {
                                return (htgo.Properties.Contains(Property.Track)
                                && htgo.Properties.Contains(Property.Aspect)
                                && htgo.Properties.Contains(Property.English));
                            });

                    options.Cast<AspectTrack>().ToList().ForEach(t => t.AddToValue(2));
                });
        }
    }
}
