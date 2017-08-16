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
            SelectionEventChoices.Add(
                (Game game, IChoiceHandler choiceHandler) =>
                {
                    List<Jury.JuryAspect> juryAspects = new List<Jury.JuryAspect>();

                    List<HTGameObject> options = game.GetHTGOFromCondition(
                            (HTGameObject htgo) =>
                            {
                                return (htgo.Properties.Contains(Property.Jury)
                                && htgo.Properties.Contains(Property.Aspect)
                                && htgo.Properties.Contains(Property.Occupation)
                                && !((Jury.JuryAspect)htgo).IsRevealed);
                            });

                    BoardChoices choices = new BoardChoices();
                    choices.NotCancelled = choiceHandler.ChooseJuryAspects(new List<List<HTGameObject>>() { options }, new List<int>() { 3 },
                        game, out choices.JuryAspects);

                    return choices;
                });

            SelectionEvents.Add(revealAllAspects);

            SelectionEventChoices.Add(
                (Game game, IChoiceHandler choiceHandler) =>
                {
                    List<Jury.JuryAspect> juryAspects = new List<Jury.JuryAspect>();

                    List<HTGameObject> options = game.GetHTGOFromCondition(
                            (HTGameObject htgo) =>
                            {
                                return (htgo.Properties.Contains(Property.Jury)
                                && htgo.Properties.Contains(Property.Aspect)
                                && htgo.Properties.Contains(Property.Religion)
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
            TrialEventChoices.Add(
                (Game game, IChoiceHandler choiceHandler) =>
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
                    List<HTGameObject> options = game.GetHTGOFromCondition(
                            (HTGameObject htgo) =>
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
