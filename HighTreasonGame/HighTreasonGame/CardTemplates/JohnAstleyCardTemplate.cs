using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                (int gameId, IChoiceHandler choiceHandler) => {
                    List<HTGameObject> options = Game.GetGameFromId(gameId).GetHTGOFromCondition(
                        (HTGameObject htgo) =>
                        {
                            return (htgo.properties.Contains(Property.Jury)
                            && htgo.properties.Contains(Property.Aspect)
                            && htgo.properties.Contains(Property.Religion));
                        });

                    BoardChoices choices = new BoardChoices();
                    choices.juryAspects = choiceHandler.ChooseJuryAspects(options, 2);

                    return choices; // Temp.
                });

            SelectionEvents.Add(
                (int gameId, BoardChoices choices) => {
                    choices.juryAspects.ForEach(ja => ja.Revealed());
                });

            SelectionEventChoices.Add(
                (int gameId, IChoiceHandler IChoiceHandler) =>
                {
                    List<HTGameObject> options = Game.GetGameFromId(gameId).GetHTGOFromCondition(
                        (HTGameObject htgo) =>
                        {
                            return (htgo.properties.Contains(Property.Jury)
                            && htgo.properties.Contains(Property.Aspect)
                            && htgo.properties.Contains(Property.Occupation));
                        });

                    BoardChoices choices = new BoardChoices();
                    choices.juryAspects = IChoiceHandler.ChooseJuryAspects(options, 3);

                    return choices;
                });

            SelectionEvents.Add(
                (int gameId, BoardChoices choices) => 
                {
                    choices.juryAspects.ForEach(ja => ja.Revealed());
                });
        }

        protected override void addTrialEventsAndChoices()
        {
            TrialEventChoices.Add(
                (int gameId, IChoiceHandler choiceHandler) =>
                {
                    Game game = Game.GetGameFromId(gameId);

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
                (int gameId, BoardChoices choices) => 
                {
                    choices.evidenceTracks[0].AddToValue(1);
                    choices.aspectTracks.ForEach(t => t.AddToValue(1));
                });
        }

        protected override void addSummationEventsAndChoices()
        {
            SummationEventChoices.Add(
                (int gameId, IChoiceHandler choiceHandler) =>
                {
                    Game game = Game.GetGameFromId(gameId);

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
                (int gameId, BoardChoices choices) => 
                {
                    choices.aspectTracks.ForEach(t => t.AddToValue(1));
                });
        }
    }
}
