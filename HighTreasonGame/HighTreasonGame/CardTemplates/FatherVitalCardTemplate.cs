using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame.CardTemplates
{
    public class FatherVitalCardTemplate : CardTemplate
    {
        public FatherVitalCardTemplate()
            : base("Father Vital Fourmond", 2)
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

            SelectionEventChoices.Add(
                (Game game, ChoiceHandler choiceHandler) =>
                {
                    List<Jury.JuryAspect> juryAspects = new List<Jury.JuryAspect>();

                    List<HTGameObject> options = game.GetHTGOFromCondition(
                            (HTGameObject htgo) =>
                            {
                                return (htgo.Properties.Contains(Property.Jury)
                                && htgo.Properties.Contains(Property.Aspect)
                                && htgo.Properties.Contains(Property.Religion)
                                && !((Jury.JuryAspect)htgo).IsVisibleToPlayer(game.CurPlayer.Side));
                            });

                    BoardChoices choices = new BoardChoices();
                    choices.NotCancelled = choiceHandler.ChooseJuryAspects(new List<List<HTGameObject>>() { options }, new List<int>() { 1 },
                        game, out choices.JuryAspects);

                    return choices;
                });

            SelectionEvents.Add(peekAllAspects);
        }

        protected override void addTrialEventsAndChoices()
        {
            TrialEventChoices.Add(doNothingChoice);

            TrialEvents.Add(
                (Game game, BoardChoices choices) =>
                {
                    game.GetInsanityTrack().AddToValue(1);
                });

            TrialEventChoices.Add(
                (Game game, ChoiceHandler choiceHandler) =>
                {
                    List<Jury.JuryAspect> juryAspects = new List<Jury.JuryAspect>();

                    List<HTGameObject> options = game.GetHTGOFromCondition(
                            (HTGameObject htgo) =>
                            {
                                return (htgo.Properties.Contains(Property.Jury)
                                && htgo.Properties.Contains(Property.Aspect)
                                && htgo.Properties.Contains(Property.Religion)
                                && !((Jury.JuryAspect)htgo).IsVisibleToPlayer(game.CurPlayer.Side));
                            });

                    BoardChoices choices = new BoardChoices();
                    choices.NotCancelled = choiceHandler.ChooseJuryAspects(new List<List<HTGameObject>>() { options }, new List<int>() { 3 },
                        game, out choices.JuryAspects);

                    return choices;
                });

            TrialEvents.Add(peekAllAspects);
        }

        protected override void addSummationEventsAndChoices()
        {
            SummationEventChoices.Add(doNothingChoice);

            SummationEvents.Add(
                (Game game, BoardChoices choices) =>
                {
                    game.GetInsanityTrack().AddToValue(1);
                });
        }
    }
}
