using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                (Game game, IChoiceHandler choiceHandler) =>
                {
                    List<Jury.JuryAspect> juryAspects = new List<Jury.JuryAspect>();

                    List<HTGameObject> religionChoices = game.GetHTGOFromCondition(
                            (HTGameObject htgo) =>
                            {
                                return (htgo.Properties.Contains(Property.Jury)
                                && htgo.Properties.Contains(Property.Aspect)
                                && htgo.Properties.Contains(Property.Religion)
                                && !((Jury.JuryAspect)htgo).IsRevealed);
                            });
                    List<HTGameObject> languageChoices = game.GetHTGOFromCondition(
                            (HTGameObject htgo) =>
                            {
                                return (htgo.Properties.Contains(Property.Jury)
                                && htgo.Properties.Contains(Property.Aspect)
                                && htgo.Properties.Contains(Property.Language)
                                && !((Jury.JuryAspect)htgo).IsRevealed);
                            });
                    List<HTGameObject> occupationChoices = game.GetHTGOFromCondition(
                            (HTGameObject htgo) =>
                            {
                                return (htgo.Properties.Contains(Property.Jury)
                                && htgo.Properties.Contains(Property.Aspect)
                                && htgo.Properties.Contains(Property.Occupation)
                                && !((Jury.JuryAspect)htgo).IsRevealed);
                            });

                    BoardChoices choices = new BoardChoices();
                    choices.NotCancelled = choiceHandler.ChooseJuryAspects(new List<List<HTGameObject>>() { religionChoices, languageChoices, occupationChoices },
                        new List<int>() { 1, 1, 1 }, 
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
                    choices.NotCancelled = handleMomentOfInsightChoice(new List<Player.PlayerSide>() { Player.PlayerSide.Defense }, game, choiceHandler, out choices.MoIInfo);
                    return choices;
                });

            TrialEvents.Add(
                (Game game, BoardChoices choices) => 
                {
                    game.GetInsanityTrack().AddToValue(1);
                    handleMomentOfInsight(game, choices);
                });

            TrialEventChoices.Add(
                (Game game, IChoiceHandler choiceHandler) =>
                {
                    BoardChoices choices = new BoardChoices();

                    List<HTGameObject> options = game.GetHTGOFromCondition(
                        (HTGameObject htgo) =>
                        {
                            return (htgo.Properties.Contains(Property.Track)
                            && htgo.Properties.Contains(Property.Aspect)
                            && (htgo.Properties.Contains(Property.Farmer) || htgo.Properties.Contains(Property.French))
                            && ((Track)htgo).CanModify(-1));
                        });

                    choices.AspectTracks = options.Cast<AspectTrack>().ToList();

                    return choices;
                });

            TrialEvents.Add(
                (Game game, BoardChoices choices) => 
                {
                    choices.AspectTracks.ForEach(t => t.AddToValue(-2));
                });
        }

        protected override void addSummationEventsAndChoices()
        {
            SummationEventChoices.Add(
                (Game game, IChoiceHandler choiceHandler) =>
                {                    
                    BoardChoices choices = new BoardChoices();
                    return choices;
                });

            SummationEvents.Add(
                (Game game, BoardChoices choices) => 
                {
                    game.GetInsanityTrack().AddToValue(1);
                });
        }
    }
}
