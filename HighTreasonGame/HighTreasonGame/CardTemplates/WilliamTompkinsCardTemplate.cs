﻿using System;
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
            SelectionEventChoices.Add(genRevealOrPeakCardChoice(new HashSet<Property> { Property.Occupation }, 3, true));
            SelectionEvents.Add(revealAllAspects);

            SelectionEventChoices.Add(genRevealOrPeakCardChoice(new HashSet<Property> { Property.Language }, 2, true));
            SelectionEvents.Add(revealAllAspects);

            SelectionEventChoices.Add(genRevealOrPeakCardChoice(new HashSet<Property>() { Property.Religion }, 1, true));
            SelectionEvents.Add(revealAllAspects);
        }

        protected override void addTrialEventsAndChoices()
        {
            TrialEventChoices.Add(
                (Game game, ChoiceHandler choiceHandler) =>
                {
                    int modValue = calcModValueBasedOnSide(2, game);

                    List<BoardObject> options = game.FindBO(
                        (Type t) =>
                        {
                            return (t == typeof(AspectTrack));
                        },
                        (BoardObject htgo) =>
                        {
                            return (htgo.Properties.Contains(Property.Track)
                            && htgo.Properties.Contains(Property.Aspect)
                            && htgo.Properties.Contains(Property.Occupation)
                            && ((Track)htgo).CanModify(modValue));
                        });

                    BoardChoices boardChoices;
                    choiceHandler.ChooseBoardObjects(
                        options,
                        (Dictionary<BoardObject, int> selected) => { return true; },
                        (List<BoardObject> remainingChoices, Dictionary<BoardObject, int> selected) =>
                        {
                            return remainingChoices.Where(obj => !selected.ContainsKey(obj)).ToList();
                        },
                        (Dictionary<BoardObject, int> selected) => { return selected.Keys.Count == 1; },
                        game,
                        out boardChoices);
                    
                    if (boardChoices.NotCancelled)
                    {
                        boardChoices.NotCancelled = handleMomentOfInsightChoice(new List<Player.PlayerSide>() { Player.PlayerSide.Prosecution, Player.PlayerSide.Defense },
                            game, choiceHandler, out boardChoices.MoIInfo);
                    }

                    return boardChoices;
                });

            TrialEvents.Add(
                (Game game, BoardChoices choices) =>
                {
                    int modValue = calcModValueBasedOnSide(2, game);
                    choices.SelectedObjs.Keys.Cast<AspectTrack>().ToList().ForEach(t => t.AddToValue(modValue));
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

                    List<AspectTrack> options = game.FindBO(
                        (Type t) =>
                        {
                            return (t == typeof(AspectTrack));
                        },
                        (BoardObject htgo) =>
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
