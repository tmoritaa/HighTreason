using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighTreasonGame.CardTemplates
{
    public class PurelyConstitutionalCardTemplate : CardTemplate
    {
        public PurelyConstitutionalCardTemplate()
            : base("\"A Purely Constitutional Movement\"", 3)
        {}

        protected override void addSelectionEventsAndChoices()
        {
            SelectionEventChoices.Add(
                (int gameId, IChoiceHandler choiceHandler) =>
                {
                    List<Jury.JuryAspect> juryAspects = new List<Jury.JuryAspect>();

                    choiceHandler.ChooseJuryAspects(
                        Game.GetGameFromId(gameId).GetHTGOFromCondition(
                            (HTGameObject htgo) =>
                            {
                                return (htgo.properties.Contains(Property.Jury)
                                && htgo.properties.Contains(Property.Aspect)
                                && htgo.properties.Contains(Property.Religion)
                                && !((Jury.JuryAspect)htgo).IsFullyRevealed);
                            }),
                        1).ForEach(a => juryAspects.Add(a));

                    choiceHandler.ChooseJuryAspects(
                        Game.GetGameFromId(gameId).GetHTGOFromCondition(
                            (HTGameObject htgo) =>
                            {
                                return (htgo.properties.Contains(Property.Jury)
                                && htgo.properties.Contains(Property.Aspect)
                                && htgo.properties.Contains(Property.Language)
                                && !((Jury.JuryAspect)htgo).IsFullyRevealed);
                            }),
                        1).ForEach(a => juryAspects.Add(a));

                    choiceHandler.ChooseJuryAspects(
                        Game.GetGameFromId(gameId).GetHTGOFromCondition(
                            (HTGameObject htgo) =>
                            {
                                return (htgo.properties.Contains(Property.Jury)
                                && htgo.properties.Contains(Property.Aspect)
                                && htgo.properties.Contains(Property.Occupation)
                                && !((Jury.JuryAspect)htgo).IsFullyRevealed);
                            }),
                        1).ForEach(a => juryAspects.Add(a));


                    BoardChoices choices = new BoardChoices();
                    choices.juryAspects = juryAspects;

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
                    BoardChoices choices = new BoardChoices();
                    choices.evidenceTracks.Add(findInsanityTrack(Game.GetGameFromId(gameId)));
                    return null;
                });

            TrialEvents.Add(
                (int gameId, BoardChoices choices) => 
                {
                    Game game = Game.GetGameFromId(gameId);
                    choices.evidenceTracks[0].AddToValue(1);
                    handleMomentOfInsight(game);
                });

            TrialEventChoices.Add(
                (int gameId, IChoiceHandler choiceHandler) =>
                {
                    Game game = Game.GetGameFromId(gameId);

                    BoardChoices choices = new BoardChoices();

                    List<HTGameObject> options = game.GetHTGOFromCondition(
                        (HTGameObject htgo) =>
                        {
                            return (htgo.properties.Contains(Property.Track)
                            && htgo.properties.Contains(Property.Aspect)
                            && (htgo.properties.Contains(Property.Farmer) || htgo.properties.Contains(Property.French))
                            && ((Track)htgo).CanDecrease());
                        });

                    choices.aspectTracks = options.Cast<AspectTrack>().ToList();

                    return choices;
                });

            TrialEvents.Add(
                (int gameId, BoardChoices choices) => 
                {
                    choices.aspectTracks.ForEach(t => t.AddToValue(-2));
                });
        }

        protected override void addSummationEventsAndChoices()
        {
            SummationEventChoices.Add(
                (int gameId, IChoiceHandler choiceHandler) =>
                {
                    Game game = Game.GetGameFromId(gameId);
                    BoardChoices choices = new BoardChoices();
                    choices.evidenceTracks.Add(findInsanityTrack(game));

                    return choices;
                });

            SummationEvents.Add(
                (int gameId, BoardChoices choices) => 
                {
                    choices.evidenceTracks[0].AddToValue(1);
                });
        }
    }
}
