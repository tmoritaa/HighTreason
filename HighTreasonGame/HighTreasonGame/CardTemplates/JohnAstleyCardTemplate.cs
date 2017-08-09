﻿using System;
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
                (Game game, IChoiceHandler IChoiceHandler) =>
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
                    choices.JuryAspects = IChoiceHandler.ChooseJuryAspects(new List<List<HTGameObject>>() { options }, new List<int>() { 3 }, game);

                    return choices;
                });

            SelectionEvents.Add(
                (Game game, BoardChoices choices) => 
                {
                    choices.JuryAspects.ForEach(ja => ja.Revealed());
                });

            SelectionEventChoices.Add(
                (Game game, IChoiceHandler choiceHandler) => {
                    List<HTGameObject> options = game.GetHTGOFromCondition(
                        (HTGameObject htgo) =>
                        {
                            return (htgo.Properties.Contains(Property.Jury)
                            && htgo.Properties.Contains(Property.Aspect)
                            && htgo.Properties.Contains(Property.Religion)
                            && !((Jury.JuryAspect)htgo).IsRevealed);
                        });

                    BoardChoices choices = new BoardChoices();
                    choices.JuryAspects = choiceHandler.ChooseJuryAspects(new List<List<HTGameObject>>() { options }, new List<int>() { 2 }, game);

                    return choices;
                });

            SelectionEvents.Add(
                (Game game, BoardChoices choices) => {
                    choices.JuryAspects.ForEach(ja => ja.Revealed());
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
                            return (htgo.Properties.Contains(Property.Track)
                            && htgo.Properties.Contains(Property.Aspect)
                            && ((Track)htgo).CanIncrease());
                        });

                    BoardChoices choices = new BoardChoices();
                    choices.EvidenceTracks.Add(findGuiltTrack(game));
                    choices.AspectTracks = choiceHandler.ChooseAspectTracks(options, 1, game);

                    return choices;
                });

            TrialEvents.Add(
                (Game game, BoardChoices choices) => 
                {
                    if (choices.EvidenceTracks.Count > 0)
                    {
                        choices.EvidenceTracks[0].AddToValue(1);
                    }
                    
                    choices.AspectTracks.ForEach(t => t.AddToValue(1));
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
                            return (htgo.Properties.Contains(Property.Track)
                            && htgo.Properties.Contains(Property.Aspect)
                            && !htgo.Properties.Contains(Property.French)
                            && ((Track)htgo).CanIncrease());
                        });

                    BoardChoices choices = new BoardChoices();
                    choices.AspectTracks = choiceHandler.ChooseAspectTracks(options, 3, game);

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
