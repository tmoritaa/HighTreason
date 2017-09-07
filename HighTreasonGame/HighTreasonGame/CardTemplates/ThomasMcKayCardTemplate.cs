using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame.CardTemplates
{
    public class ThomasMcKayCardTemplate : CardTemplate
    {
        public ThomasMcKayCardTemplate()
            : base("Thomas McKay", 2)
        { }

        protected override void addSelectionEventsAndChoices()
        {
            SelectionEventChoices.Add(genRevealOrPeakCardChoice(new HashSet<Property>(), 2, true, null,
                (List<BoardObject> remainingChoices, Dictionary<BoardObject, int> selected) =>
                {
                    List<BoardObject> newChoices = new List<BoardObject>(remainingChoices);
                    foreach (BoardObject obj in selected.Keys)
                    {
                        if (obj.Properties.Contains(Property.Religion))
                        {
                            newChoices = newChoices.Where(c => !c.Properties.Contains(Property.Religion)).ToList();
                        }

                        if (obj.Properties.Contains(Property.Occupation))
                        {
                            newChoices = newChoices.Where(c => !c.Properties.Contains(Property.Occupation)).ToList();
                        }

                        if (obj.Properties.Contains(Property.Language))
                        {
                            newChoices = newChoices.Where(c => !c.Properties.Contains(Property.Language)).ToList();
                        }
                    }

                    return newChoices;
                }));
            SelectionEvents.Add(revealAllAspects);
        }

        protected override void addTrialEventsAndChoices()
        {
            TrialEventChoices.Add(genAspectTrackForModCardChoice(new HashSet<Property>(), 1, 1, false));
            TrialEvents.Add(raiseGuiltAndOneAspectEffect);
        }

        protected override void addSummationEventsAndChoices()
        {
            SummationEventChoices.Add(doNothingChoice);

            SummationEvents.Add(
                (Game game, BoardChoices choices) =>
                {
                    List<BoardObject> options = game.GetHTGOFromCondition(
                            (BoardObject htgo) =>
                            {
                                return (htgo.Properties.Contains(Property.Track)
                                && htgo.Properties.Contains(Property.Aspect)
                                && htgo.Properties.Contains(Property.English));
                            });

                    options.Cast<AspectTrack>().ToList().ForEach(t => t.AddToValue(2));
                });
        }
    }
}
