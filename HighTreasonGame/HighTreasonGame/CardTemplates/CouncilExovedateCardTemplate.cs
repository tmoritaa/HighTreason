using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame.CardTemplates
{
    public class CouncilExovedateCardTemplate : CardTemplate
    {
        public CouncilExovedateCardTemplate()
            : base("Council of the Exovedate", 2)
        {}

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
                                && !((Jury.JuryAspect)htgo).IsRevealed);
                            });

                    BoardChoices choices = new BoardChoices();
                    choices.NotCancelled = choiceHandler.ChooseJuryAspects(new List<List<HTGameObject>>() { options }, new List<int>() { 2 },
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
                    int modValue = calcModValueBasedOnSide(2, game);

                    AspectTrack aspectTrack = (AspectTrack)game.GetHTGOFromCondition(
                            (HTGameObject htgo) =>
                            {
                                return (htgo.Properties.Contains(Property.Track)
                                && htgo.Properties.Contains(Property.Aspect)
                                && htgo.Properties.Contains(Property.French));
                            })[0];

                    aspectTrack.AddToValue(modValue);
                });

            TrialEventChoices.Add(doNothingChoice);

            TrialEvents.Add(
                (Game game, BoardChoices choices) =>
                {
                    int modValue = calcModValueBasedOnSide(2, game);

                    AspectTrack aspectTrack = (AspectTrack)game.GetHTGOFromCondition(
                            (HTGameObject htgo) =>
                            {
                                return (htgo.Properties.Contains(Property.Track)
                                && htgo.Properties.Contains(Property.Aspect)
                                && htgo.Properties.Contains(Property.English));
                            })[0];

                    aspectTrack.AddToValue(modValue);
                });

            TrialEventChoices.Add(
                (Game game, IChoiceHandler choiceHandler) =>
                {
                    BoardChoices choices = new BoardChoices();
                    
                    AspectTrack aspectTrack = (AspectTrack)game.GetHTGOFromCondition(
                            (HTGameObject htgo) =>
                            {
                                return (htgo.Properties.Contains(Property.Track)
                                && htgo.Properties.Contains(Property.Aspect)
                                && htgo.Properties.Contains(Property.Merchant));
                            })[0];

                    choices.AspectTracks.Add(aspectTrack);

                    return choices;
                });

            TrialEvents.Add(
                (Game game, BoardChoices choices) =>
                {
                    int modValue = calcModValueBasedOnSide(1, game);

                    AspectTrack aspectTrack = (AspectTrack)game.GetHTGOFromCondition(
                            (HTGameObject htgo) =>
                            {
                                return (htgo.Properties.Contains(Property.Track)
                                && htgo.Properties.Contains(Property.Aspect)
                                && htgo.Properties.Contains(Property.Merchant));
                            })[0];

                    aspectTrack.AddToValue(modValue);
                });
        }

        protected override void addSummationEventsAndChoices()
        {
            SummationEventChoices.Add(doNothingChoice);

            SummationEvents.Add(
                (Game game, BoardChoices choices) =>
                {
                    int modValue = calcModValueBasedOnSide(2, game);

                    AspectTrack aspectTrack = (AspectTrack)game.GetHTGOFromCondition(
                            (HTGameObject htgo) =>
                            {
                                return (htgo.Properties.Contains(Property.Track)
                                && htgo.Properties.Contains(Property.Aspect)
                                && htgo.Properties.Contains(Property.French));
                            })[0];

                    aspectTrack.AddToValue(modValue);
                });

            SummationEventChoices.Add(doNothingChoice);

            SummationEvents.Add(
                (Game game, BoardChoices choices) =>
                {
                    int modValue = calcModValueBasedOnSide(2, game);

                    AspectTrack aspectTrack = (AspectTrack)game.GetHTGOFromCondition(
                            (HTGameObject htgo) =>
                            {
                                return (htgo.Properties.Contains(Property.Track)
                                && htgo.Properties.Contains(Property.Aspect)
                                && htgo.Properties.Contains(Property.English));
                            })[0];

                    aspectTrack.AddToValue(modValue);
                });

            SummationEventChoices.Add(
                (Game game, IChoiceHandler choiceHandler) =>
                {
                    BoardChoices choices = new BoardChoices();

                    AspectTrack aspectTrack = (AspectTrack)game.GetHTGOFromCondition(
                            (HTGameObject htgo) =>
                            {
                                return (htgo.Properties.Contains(Property.Track)
                                && htgo.Properties.Contains(Property.Aspect)
                                && htgo.Properties.Contains(Property.Merchant));
                            })[0];

                    choices.AspectTracks.Add(aspectTrack);

                    return choices;
                });

            SummationEvents.Add(
                (Game game, BoardChoices choices) =>
                {
                    int modValue = calcModValueBasedOnSide(1, game);

                    AspectTrack aspectTrack = (AspectTrack)game.GetHTGOFromCondition(
                            (HTGameObject htgo) =>
                            {
                                return (htgo.Properties.Contains(Property.Track)
                                && htgo.Properties.Contains(Property.Aspect)
                                && htgo.Properties.Contains(Property.Merchant));
                            })[0];

                    aspectTrack.AddToValue(modValue);
                });
        }
    }
}
