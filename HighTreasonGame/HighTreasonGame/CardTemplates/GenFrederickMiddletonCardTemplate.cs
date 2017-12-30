using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json.Linq;

namespace HighTreasonGame
{
    [CardTemplateAttribute]
    public class GenFrederickMiddletonCardTemplate : CardTemplate
    {
        public GenFrederickMiddletonCardTemplate()
            : base("Gen. Frederick Middleton", 3, Player.PlayerSide.Prosecution)
        { }

        protected override void addSelectionEventsAndChoices()
        {
            SelectionEvents.Add(new CardEffectPair(
                genRevealOrPeakCardChoice(new HashSet<Property>(), 3, true, this.CardInfo.JurySelectionInfos[0].Description,
                    null,
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
                    },
                    LimitNumAspectFilterComb(1)),
                revealAllAspects));
        }

        protected override void addTrialEventsAndChoices()
        {
            TrialEvents.Add(
                new CardEffectPair(
                    doNothingChoice,
                    (Game game, Player choosingPlayer, BoardChoices choices) =>
                    {
                        game.Board.GetGuiltTrack().AddToValue(1);

                        findAspectTracksWithProp(game, Property.English, Property.GovWorker).ForEach(t => t.AddToValue(t.Properties.Contains(Property.English) ? 2 : 1));

                        game.OfficersRecalledPlayable = true;
                    }));
        }

        protected override void addSummationEventsAndChoices()
        {
            SummationEvents.Add(
                new CardEffectPair(
                    doNothingChoice,
                    (Game game, Player choosingPlayer, BoardChoices choices) =>
                    {
                        List<AspectTrack> aspectTracks = findAspectTracksWithProp(game, Property.Occupation, Property.English, Property.Catholic);
                        aspectTracks.ForEach(t => t.AddToValue((t.Properties.Contains(Property.Occupation) || t.Properties.Contains(Property.English)) ? 2 : 1));

                        game.OfficersRecalledPlayable = true;
                    }));
        }
    }
}
