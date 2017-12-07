using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame
{
    [CardTemplateAttribute]
    public class GeorgeBurbidgeCardTemplate : CardTemplate
    {
        public GeorgeBurbidgeCardTemplate() 
            : base("George W. Burbidge", 2, Player.PlayerSide.Prosecution, true)
        { }

        protected override void addSelectionEventsAndChoices()
        {
            SelectionEvents.Add(
                new CardEffectPair(
                    (Game game, Player choosingPlayer, ChoiceHandler handler) =>
                    {
                        int numChoices = (game.CurPlayer.Side == side) ? 2 : 1;

                        CardChoice func = genRevealOrPeakCardChoice(new HashSet<Property>() {}, numChoices, false, this.CardInfo.JurySelectionInfos[0].Description);
                        return func(game, choosingPlayer, handler);
                    },
                    peekAllAspects));
        }

        protected override void addTrialEventsAndChoices()
        {
            TrialEvents.Add(
                new CardEffectPair(
                    doNothingChoice,
                    doNothingEffect,
                    (Game game) => { return false; }));

            TrialEvents.Add(
                new CardEffectPair(
                    (Game game, Player choosingPlayer, ChoiceHandler choiceHandler) =>
                    {
                        List<AspectTrack> tracks = findAspectTracksWithProp(game);

                        BoardChoices boardChoices;
                        choiceHandler.ChooseBoardObjects(
                            tracks.Cast<BoardObject>().ToList(),
                            (Dictionary<BoardObject, int> selected) => { return true; },
                            (List<BoardObject> choices, Dictionary<BoardObject, int> selected) =>
                            {
                                return choices;
                            },
                            (Dictionary<BoardObject, int> selected) =>
                            {
                                return selected.Count == 1;
                            },
                            game,
                            choosingPlayer,
                            this.CardInfo.TrialInChiefInfos[1].Description,
                            out boardChoices);

                        return boardChoices;
                    },
                    (Game game, BoardChoices choices) =>
                    {
                        AspectTrack track = (AspectTrack)choices.SelectedObjs.Keys.First();

                        track.ResetTimesAffected();
                        track.AddToValue(calcModValueBasedOnSide(2, game));
                    }));

            TrialEvents.Add(
                new CardEffectPair(
                    (Game game, Player choosingPlayer, ChoiceHandler choiceHandler) =>
                    {
                        List<BoardObject> bos = game.FindBO(
                            (BoardObject bo) =>
                            {
                                return
                                    bo.Properties.Contains(Property.Sway)
                                    && bo.Properties.Contains(Property.Track)
                                    && bo.Properties.Contains(Property.Jury);
                            });

                        BoardChoices boardChoices;
                        choiceHandler.ChooseBoardObjects(
                            bos,
                            (Dictionary<BoardObject, int> selected) =>
                            {
                                int actionPtsLeft = ActionPts - HTUtility.CalcActionPtUsage(selected);
                                return (actionPtsLeft >= 0);
                            },
                            (List<BoardObject> choicesLeft, Dictionary<BoardObject, int> selected) =>
                            {
                                return choicesLeft.FindAll(t => 
                                    (choosingPlayer.Side == Player.PlayerSide.Prosecution) ? !((SwayTrack)t).IsLockedByProsecution : !((SwayTrack)t).IsLockedByDefense);
                            },
                            (Dictionary<BoardObject, int> selected) =>
                            {
                                int actionPtsLeft = ActionPts - HTUtility.CalcActionPtUsage(selected);
                                return (actionPtsLeft == 0);
                            },
                            game,
                            choosingPlayer,
                            this.CardInfo.TrialInChiefInfos[2].Description,
                            out boardChoices);

                        return boardChoices;
                    },
                    (Game game, BoardChoices boardChoices) =>
                    {
                        int sideMod = (game.CurPlayer.Side == Player.PlayerSide.Prosecution) ? 1 : -1;
                        foreach (KeyValuePair<BoardObject, int> kv in boardChoices.SelectedObjs)
                        {
                            SwayTrack track = (SwayTrack)kv.Key;
                            track.AddToValue(sideMod * kv.Value);
                        }
                    }));
        }

        protected override void addSummationEventsAndChoices()
        {
            SummationEvents.Add(
                new CardEffectPair(doNothingChoice, doNothingEffect, (Game game) => { return false; }));

            SummationEvents.Add(
                new CardEffectPair(
                    (Game game, Player choosingPlayer, ChoiceHandler choiceHandler) =>
                    {
                        List<BoardObject> bos = game.FindBO(
                            (BoardObject bo) =>
                            {
                                return bo.Properties.Contains(Property.Sway)
                                    && bo.Properties.Contains(Property.Track)
                                    && bo.Properties.Contains(Property.Jury)
                                    && !((SwayTrack)bo).IsLocked;
                            });

                        BoardChoices boardChoices;
                        choiceHandler.ChooseBoardObjects(
                            bos,
                            (Dictionary<BoardObject, int> selected) =>
                            {
                                int actionPtsLeft = ActionPts - HTUtility.CalcActionPtUsage(selected);
                                return (actionPtsLeft >= 0);
                            },
                            (List<BoardObject> choicesLeft, Dictionary<BoardObject, int> selected) =>
                            {
                                return choicesLeft.FindAll(t => !((SwayTrack)t).IsLocked);
                            },
                            (Dictionary<BoardObject, int> selected) =>
                            {
                                int actionPtsLeft = ActionPts - HTUtility.CalcActionPtUsage(selected);
                                return (actionPtsLeft == 0);
                            },
                            game,
                            choosingPlayer,
                            this.CardInfo.SummationInfos[1].Description,
                            out boardChoices);

                        return boardChoices;
                    },
                    (Game game, BoardChoices boardChoices) =>
                    {
                        int sideMod = (game.CurPlayer.Side == Player.PlayerSide.Prosecution) ? 1 : -1;
                        foreach (KeyValuePair<BoardObject, int> kv in boardChoices.SelectedObjs)
                        {
                            SwayTrack track = (SwayTrack)kv.Key;
                            track.AddToValue(sideMod * kv.Value);
                        }
                    }));

            SummationEvents.Add(
                new CardEffectPair(
                    (Game game, Player choosingPlayer, ChoiceHandler choiceHandler) =>
                    {
                        List<BoardObject> bos = game.FindBO(
                            (BoardObject bo) =>
                            {
                                return bo.Properties.Contains(Property.Sway)
                                    && bo.Properties.Contains(Property.Track)
                                    && bo.Properties.Contains(Property.Jury);
                            });

                        BoardChoices boardChoices;
                        choiceHandler.ChooseBoardObjects(
                            bos,
                            (Dictionary<BoardObject, int> selected) => { return true; },
                            (List<BoardObject> choicesLeft, Dictionary<BoardObject, int> selected) => { return choicesLeft; },
                            (Dictionary<BoardObject, int> selected) =>
                            {
                                return selected.Count == 1;
                            },
                            game,
                            choosingPlayer,
                            this.CardInfo.SummationInfos[2].Description,
                            out boardChoices);

                        return boardChoices;
                    },
                    (Game game, BoardChoices boardChoices) =>
                    {
                        SwayTrack track = (SwayTrack)boardChoices.SelectedObjs.Keys.First();
                        track.ResetValue();
                        track.AddToValue((game.CurPlayer.Side == Player.PlayerSide.Prosecution) ? 1 : -1);
                    }));
        }
    }
}
