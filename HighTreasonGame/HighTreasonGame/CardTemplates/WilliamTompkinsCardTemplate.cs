using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame.CardTemplates
{
    public class WilliamTompkinsCardTemplate : CardTemplate
    {
        public WilliamTompkinsCardTemplate()
            : base("William Tompkins", 3)
        { }

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
                (Game game, ChoiceHandler choiceHandler) =>
                {
                    List<HTGameObject> options = game.GetHTGOFromCondition(
                        (HTGameObject htgo) =>
                        {
                            return (htgo.Properties.Contains(Property.Jury)
                            && htgo.Properties.Contains(Property.Aspect)
                            && htgo.Properties.Contains(Property.Language)
                            && !((Jury.JuryAspect)htgo).IsRevealed);
                        });

                    BoardChoices choices = new BoardChoices();
                    choices.NotCancelled = choiceHandler.ChooseJuryAspects(new List<List<HTGameObject>>() { options }, new List<int>() { 2 }, game, out choices.JuryAspects);

                    return choices;
                });

            SelectionEvents.Add(revealAllAspects);

            SelectionEventChoices.Add(
                (Game game, ChoiceHandler choiceHandler) =>
                {
                    List<HTGameObject> options = game.GetHTGOFromCondition(
                        (HTGameObject htgo) =>
                        {
                            return (htgo.Properties.Contains(Property.Jury)
                            && htgo.Properties.Contains(Property.Aspect)
                            && htgo.Properties.Contains(Property.Religion)
                            && !((Jury.JuryAspect)htgo).IsRevealed);
                        });

                    BoardChoices choices = new BoardChoices();
                    choices.NotCancelled = choiceHandler.ChooseJuryAspects(new List<List<HTGameObject>>() { options }, new List<int>() { 1 }, game, out choices.JuryAspects);

                    return choices;
                });

            SelectionEvents.Add(revealAllAspects);
        }

        protected override void addTrialEventsAndChoices()
        {
            TrialEventChoices.Add(
                (Game game, ChoiceHandler choiceHandler) =>
                {
                    int modValue = calcModValueBasedOnSide(2, game);

                    List<HTGameObject> options = game.GetHTGOFromCondition(
                        (HTGameObject htgo) =>
                        {
                            return (htgo.Properties.Contains(Property.Track)
                            && htgo.Properties.Contains(Property.Aspect)
                            && htgo.Properties.Contains(Property.Occupation)
                            && ((Track)htgo).CanModify(modValue));
                        });

                    BoardChoices choices = new BoardChoices();
                    choices.NotCancelled = choiceHandler.ChooseAspectTracks(options, 1, game, out choices.AspectTracks);

                    if (choices.NotCancelled)
                    {
                        choices.NotCancelled = handleMomentOfInsightChoice(new List<Player.PlayerSide>() { Player.PlayerSide.Prosecution, Player.PlayerSide.Defense },
                            game, choiceHandler, out choices.MoIInfo);
                    }

                    return choices;
                });

            TrialEvents.Add(
                (Game game, BoardChoices choices) =>
                {
                    int modValue = calcModValueBasedOnSide(2, game);
                    choices.AspectTracks.ForEach(t => t.AddToValue(modValue));
                    handleMomentOfInsight(game, choices);
                });
        }

        protected override void addSummationEventsAndChoices()
        {
            SummationEventChoices.Add(doNothingChoice);

            SummationEvents.Add(
                (Game game, BoardChoices choices) =>
                {
                    int sign = (game.CurPlayer.Side == Player.PlayerSide.Prosecution ? 1 : -1);

                    List<AspectTrack> options = game.GetHTGOFromCondition(
                        (HTGameObject htgo) =>
                        {
                            return (htgo.Properties.Contains(Property.Track)
                            && htgo.Properties.Contains(Property.Aspect)
                            && htgo.Properties.Contains(Property.Occupation));
                        }).Cast<AspectTrack>().ToList();

                    foreach (AspectTrack track in options)
                    {
                        if (track.Properties.Contains(Property.Farmer))
                        {
                            track.AddToValue(-calcModValueBasedOnSide(1, game));
                        }
                        else
                        {
                            track.AddToValue(calcModValueBasedOnSide(2, game));
                        }
                    }
                });
        }
    }
}
