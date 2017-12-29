using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame
{
    public class HTUtility
    {
        public static int CalcActionPtUsage(Dictionary<BoardObject, int> selected)
        {
            int actionPtsUsed = 0;
            foreach (BoardObject bo in selected.Keys)
            {
                int numTimesAffected = selected[bo];

                Track track = (Track)bo;
                if (track.GetType() == typeof(SwayTrack))
                {
                    actionPtsUsed += numTimesAffected;
                }
                else if (track.GetType() == typeof(AspectTrack))
                {
                    actionPtsUsed += (numTimesAffected > 1) ? 3 : 1;
                }
                else
                {
                    System.Diagnostics.Debug.Assert(false, "Track chosen to affect by action is not sway or aspect track. Should never happen!");
                }
            }

            return actionPtsUsed;
        }

        public static Func<Dictionary<BoardObject, int>, bool> GenActionValidateChoicesFunc(int actionPts, Jury delibJury)
        {
            return
                (Dictionary<BoardObject, int> selected) =>
                {
                    bool isValid = true;
                    int actionPtsLeft = actionPts;

                    foreach (BoardObject bo in selected.Keys)
                    {
                        int affectLimit = 2;
                        if (delibJury != null)
                        {
                            affectLimit = 0;
                            foreach (Jury.JuryAspect aspect in delibJury.Aspects)
                            {
                                affectLimit += (bo.Properties.Contains(aspect.Aspect)) ? 1 : 0;
                            }
                        }

                        if (selected[bo] > affectLimit)
                        {
                            isValid = false;
                            break;
                        }
                    }

                    if (isValid)
                    {
                        actionPtsLeft -= CalcActionPtUsage(selected);
                        isValid &= (actionPtsLeft >= 0);
                    }

                    return isValid;
                };
        }

        public static Func<List<BoardObject>, Dictionary<BoardObject, int>, List<BoardObject>> GenActionFilterChoicesFunc(int actionPts, Jury delibJury)
        {
            return
                (List<BoardObject> choicesLeft, Dictionary<BoardObject, int> selected) =>
                {
                    List<BoardObject> filtChoices = new List<BoardObject>();
                    foreach (BoardObject bo in choicesLeft)
                    {
                        int affectLimit = 2;
                        if (delibJury != null)
                        {
                            affectLimit = 0;
                            foreach (Jury.JuryAspect aspect in delibJury.Aspects)
                            {
                                affectLimit += (bo.Properties.Contains(aspect.Aspect)) ? 1 : 0;
                            }
                        }

                        if (!selected.ContainsKey(bo) || selected[bo] < affectLimit)
                        {
                            filtChoices.Add(bo);
                        }
                    }

                    // Filter out any already selected aspect tracks if only 1 action pt left.
                    int actionPtsLeft = actionPts - CalcActionPtUsage(selected);
                    if (actionPtsLeft == 1)
                    {
                        filtChoices = filtChoices.Where(bo => (bo.GetType() == typeof(SwayTrack)) || (bo.GetType() == typeof(AspectTrack) && !selected.ContainsKey(bo))).ToList();
                    }

                    // Filter out any swayTracks that have been selected and by result can no longer be affected.
                    List<BoardObject> finalChoices = new List<BoardObject>();
                    foreach (BoardObject obj in filtChoices)
                    {
                        if (obj.GetType() == typeof(SwayTrack) && selected.ContainsKey(obj))
                        {
                            SwayTrack track = (SwayTrack)obj;
                            if ((selected[obj] + Math.Abs(track.Value)) < track.MaxValue)
                            {
                                finalChoices.Add(obj);
                            }
                        }
                        else
                        {
                            finalChoices.Add(obj);
                        }
                    }

                    return finalChoices;
                };
        }

        public static Func<Dictionary<BoardObject, int>, bool> GenActionChoicesCompleteFunc(int actionPts, Jury delibJury)
        {
            return
                (Dictionary<BoardObject, int> selected) =>
                {
                    bool isValid = true;
                    int actionPtsLeft = actionPts;

                    foreach (BoardObject bo in selected.Keys)
                    {
                        int affectLimit = 2;
                        if (delibJury != null)
                        {
                            affectLimit = 0;
                            foreach (Jury.JuryAspect aspect in delibJury.Aspects)
                            {
                                affectLimit += (bo.Properties.Contains(aspect.Aspect)) ? 1 : 0;
                            }
                        }

                        if (selected[bo] > affectLimit)
                        {
                            isValid = false;
                            break;
                        }
                    }

                    if (isValid)
                    {
                        actionPtsLeft -= CalcActionPtUsage(selected);
                        isValid &= (actionPtsLeft == 0);
                    }

                    return isValid;
                };
        }

        public static Func<List<BoardObject>, List<object>> GenActionCalcCombFunc(int actionPts, Player choosingPlayer)
        {
            return 
                (List<BoardObject> choices) =>
                {
                    List<AspectTrack> aspectTracks = new List<AspectTrack>();
                    List<SwayTrack> swayTracks = new List<SwayTrack>();

                    // First separate objs into aspect and sway tracks.
                    foreach (BoardObject obj in choices)
                    {
                        if (obj.GetType() == typeof(SwayTrack))
                        {
                            swayTracks.Add((SwayTrack)obj);
                        }
                        else if (obj.GetType() == typeof(AspectTrack))
                        {
                            aspectTracks.Add((AspectTrack)obj);
                        }
                        else
                        {
                            System.Diagnostics.Debug.Assert(false, "GenActionCalcCombFunc received non sway or aspect track in choices.");
                        }
                    }

                    // Next make all combs of sway tracks selectable multiple times (1 or 2 basically), and each aspect track once and find all comb out of that
                    List<BoardObject> singleAspectCombList = new List<BoardObject>(aspectTracks.Cast<BoardObject>());
                    foreach (var track in swayTracks)
                    {
                        int numModdable = Math.Abs((choosingPlayer.Side == Player.PlayerSide.Prosecution ? track.MaxValue : track.MinValue) - track.Value);
                        int numTimesSelectable = Math.Min(numModdable, Math.Min(actionPts, 2));

                        for (int i = 0; i < numTimesSelectable; ++i)
                        {
                            singleAspectCombList.Add(track);
                        }
                    }

                    List<object> resCombs = HTUtility.FindAllCombOfBoardObjs(singleAspectCombList, actionPts);

                    // Then if actionPts greater than or equal to 3, make list of combs with each track selected once, and all combs count - 3 or something without double track
                    if (actionPts >= 3)
                    {
                        foreach(var track in aspectTracks)
                        {
                            List<BoardObject> multAspectTrackCombList = new List<BoardObject>();

                            // Add track twice.
                            multAspectTrackCombList.Add(track);
                            multAspectTrackCombList.Add(track);

                            if (actionPts == 3)
                            {
                                List<object> objs = convertIEnumIEnumToBoardChoices((IEnumerable<IEnumerable<BoardObject>>) new List<List<BoardObject>>() { multAspectTrackCombList });
                                resCombs.Add(objs);
                            }
                            else
                            {
                                List<object> objs = FindAllCombOfBoardObjs(singleAspectCombList.Where(b => b != track).ToList(), actionPts - 3);
                                foreach (var obj in objs)
                                {
                                    BoardChoices bcs = (BoardChoices)obj;
                                    bcs.SelectedObjs.Add(track, 2);
                                }
                                resCombs.Add(objs);
                            }
                        }
                    }

                    return resCombs;
                };
        }

        public static List<object> FindAllCombOfBoardObjs(
            List<BoardObject> choices,
            int count, 
            Func<IEnumerable<IEnumerable<BoardObject>>, IEnumerable<IEnumerable<BoardObject>>> filterFunc = null)
        {
            if (count <= 0)
            {
                return new List<object>();
            }

            var combs = HTUtility.getCombination(choices, count);

            if (filterFunc != null)
            {
                combs = filterFunc(combs);
            }

            return convertIEnumIEnumToBoardChoices(combs);
        }

        public static List<object> FindAllCombOfCards(List<Card> choices, int count)
        {
            if (count <= 0)
            {
                return new List<object>();
            }

            var combs = HTUtility.getCombination(choices, count);

            return convertIEnumIEnumToBoardChoices(combs);
        }

        private static List<object> convertIEnumIEnumToBoardChoices(IEnumerable<IEnumerable<Card>> combs)
        {
            List<object> boardChoices = new List<object>();
            foreach (var objs in combs)
            {
                BoardChoices boardChoice = new BoardChoices();
                foreach (var bo in objs)
                {
                    if (!boardChoice.SelectedCards.ContainsKey(bo))
                    {
                        boardChoice.SelectedCards.Add(bo, 0);
                    }

                    boardChoice.SelectedCards[bo] += 1;
                }

                boardChoices.Add(boardChoice);
            }

            return boardChoices;
        }

        private static List<object> convertIEnumIEnumToBoardChoices(IEnumerable<IEnumerable<BoardObject>> combs)
        {
            List<object> boardChoices = new List<object>();
            foreach (var objs in combs)
            {
                BoardChoices boardChoice = new BoardChoices();
                foreach (var bo in objs)
                {
                    if (!boardChoice.SelectedObjs.ContainsKey(bo))
                    {
                        boardChoice.SelectedObjs.Add(bo, 0);
                    }

                    boardChoice.SelectedObjs[bo] += 1;
                }

                boardChoices.Add(boardChoice);
            }

            return boardChoices;
        }

        private static IEnumerable<IEnumerable<T>> getCombination<T>(IEnumerable<T> items, int count)
        {
            int i = 0;
            foreach (var item in items)
            {
                if (count == 1)
                {
                    yield return new T[] { item };
                }
                else
                {
                    foreach (var result in getCombination(items.Skip(i + 1), count - 1))
                    {
                        yield return new T[] { item }.Concat(result);
                    }
                }

                ++i;
            }
        }
    }
}
