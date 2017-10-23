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
    }
}
