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
                (Game game, IChoiceHandler choiceHandler) => {
                    List<HTGameObject> options = game.GetHTGOFromCondition(
                        (HTGameObject htgo) =>
                        {
                            return (htgo.properties.Contains(Property.Jury)
                            && htgo.properties.Contains(Property.Aspect)
                            && htgo.properties.Contains(Property.Religion)
                            && !((Jury.JuryAspect)htgo).IsFullyRevealed);
                        });

                    BoardChoices choices = new BoardChoices();
                    choices.juryAspects = choiceHandler.ChooseJuryAspects(options, 2);

                    return choices; // Temp.
                });

            SelectionEvents.Add(
                (Game game, BoardChoices choices) => {
                    choices.juryAspects.ForEach(ja => ja.Revealed());
                });

            SelectionEventChoices.Add(
                (Game game, IChoiceHandler IChoiceHandler) =>
                {
                    List<HTGameObject> options = game.GetHTGOFromCondition(
                        (HTGameObject htgo) =>
                        {
                            return (htgo.properties.Contains(Property.Jury)
                            && htgo.properties.Contains(Property.Aspect)
                            && htgo.properties.Contains(Property.Occupation)
                            && !((Jury.JuryAspect)htgo).IsFullyRevealed);
                        });

                    BoardChoices choices = new BoardChoices();
                    choices.juryAspects = IChoiceHandler.ChooseJuryAspects(options, 3);

                    return choices;
                });

            SelectionEvents.Add(
                (Game game, BoardChoices choices) => 
                {
                    choices.juryAspects.ForEach(ja => ja.Revealed());
                });
        }

        protected override void addTrialEventsAndChoices()
        {
            TrialEventChoices.Add(
                (Game game, IChoiceHandler choiceHandler) =>
                {
                    List<HTGameObject> options = game.GetHTGOFromCondition(
                        (HTGameObject htgo) =>
                        {
                            return (htgo.properties.Contains(Property.Track)
                            && htgo.properties.Contains(Property.Aspect)
                            && ((Track)htgo).CanIncrease());
                        });

                    BoardChoices choices = new BoardChoices();
                    choices.evidenceTracks.Add(findGuiltTrack(game));
                    choices.aspectTracks = choiceHandler.ChooseAspectTracks(options, 1);

                    return choices;
                });

            TrialEvents.Add(
                (Game game, BoardChoices choices) => 
                {
                    if (choices.evidenceTracks.Count > 0)
                    {
                        choices.evidenceTracks[0].AddToValue(1);
                    }
                    
                    choices.aspectTracks.ForEach(t => t.AddToValue(1));
                });
        }

        protected override void addSummationEventsAndChoices()
        {
            SummationEventChoices.Add(
                (Game game, IChoiceHandler choiceHandler) =>
                {
                    List<HTGameObject> options = game.GetHTGOFromCondition(
                        (HTGameObject htgo) =>
                        {
                            return (htgo.properties.Contains(Property.Track)
                            && htgo.properties.Contains(Property.Aspect)
                            && !htgo.properties.Contains(Property.French)
                            && ((Track)htgo).CanIncrease());
                        });

                    BoardChoices choices = new BoardChoices();
                    choices.aspectTracks = choiceHandler.ChooseAspectTracks(options, 3);

                    return choices;
                });

            SummationEvents.Add(
                (Game game, BoardChoices choices) => 
                {
                    choices.aspectTracks.ForEach(t => t.AddToValue(1));
                });
        }
    }
}
