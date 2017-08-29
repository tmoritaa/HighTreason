using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame.CardTemplates
{
    public class JohnAstleyCardTemplate : CardTemplate
    {
        public JohnAstleyCardTemplate() 
            : base("John W. Astley", 2)
        {}

        protected override void addSelectionEventsAndChoices()
        {
            SelectionEventChoices.Add(
                (Game game, ChoiceHandler IChoiceHandler) =>
                {
                    List<HTGameObject> options = game.GetHTGOFromCondition(
                        (HTGameObject htgo) =>
                        {
                            return (htgo.Properties.Contains(Property.Jury)
                            && htgo.Properties.Contains(Property.Aspect)
                            && htgo.Properties.Contains(Property.Occupation)
                            && !((Jury.JuryAspect)htgo).IsRevealed);
                        });

                    BoardChoices choices = new BoardChoices();
                    choices.NotCancelled = IChoiceHandler.ChooseJuryAspects(new List<List<HTGameObject>>() { options }, new List<int>() { 3 }, game, out choices.JuryAspects);

                    return choices;
                });

            SelectionEvents.Add(revealAllAspects);

            SelectionEventChoices.Add(
                (Game game, ChoiceHandler choiceHandler) => {
                    List<HTGameObject> options = game.GetHTGOFromCondition(
                        (HTGameObject htgo) =>
                        {
                            return (htgo.Properties.Contains(Property.Jury)
                            && htgo.Properties.Contains(Property.Aspect)
                            && htgo.Properties.Contains(Property.Religion)
                            && !((Jury.JuryAspect)htgo).IsRevealed);
                        });

                    BoardChoices choices = new BoardChoices();
                    choices.NotCancelled = choiceHandler.ChooseJuryAspects(new List<List<HTGameObject>>() { options }, new List<int>() { 2 }, game, out choices.JuryAspects);

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
            SummationEventChoices.Add(
                (Game game, ChoiceHandler choiceHandler) =>
                {
                    List<HTGameObject> options = game.GetHTGOFromCondition(
                        (HTGameObject htgo) =>
                        {
                            return (htgo.Properties.Contains(Property.Track)
                            && htgo.Properties.Contains(Property.Aspect)
                            && !htgo.Properties.Contains(Property.French)
                            && ((Track)htgo).CanModify(1));
                        });

                    BoardChoices choices = new BoardChoices();
                    choices.NotCancelled = choiceHandler.ChooseAspectTracks(options, 3, game, out choices.AspectTracks);

                    return choices;
                });

            SummationEvents.Add(
                (Game game, BoardChoices choices) => 
                {
                    choices.AspectTracks.ForEach(t => t.AddToValue(1));
                });
        }
    }
}
