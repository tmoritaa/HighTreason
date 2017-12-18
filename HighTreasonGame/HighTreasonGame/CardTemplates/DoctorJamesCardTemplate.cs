using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json.Linq;

namespace HighTreasonGame
{
    [CardTemplateAttribute]
    public class DoctorJamesCardTemplate : CardTemplate
    {
        public DoctorJamesCardTemplate()
            : base("Doctor James Wallace", 2, Player.PlayerSide.Prosecution)
        { }

        protected override void addSelectionEventsAndChoices()
        {
            SelectionEvents.Add(
                new CardEffectPair(
                    genRevealOrPeakCardChoice(new HashSet<Property>() { Property.Occupation }, 3, true, this.CardInfo.JurySelectionInfos[0].Description),
                    revealAllAspects));

            SelectionEvents.Add(
                new CardEffectPair(
                    genRevealOrPeakCardChoice(new HashSet<Property>() { Property.Religion }, 2, true, this.CardInfo.JurySelectionInfos[1].Description),
                    revealAllAspects));
        }

        protected override void addTrialEventsAndChoices()
        {
            TrialEvents.Add(
                new CardEffectPair(
                    (Game game, Player choosingPlayer) =>
                    {
                        return handleMomentOfInsightChoice(new Player.PlayerSide[] { Player.PlayerSide.Prosecution }, game, choosingPlayer);
                    },
                    (Game game, Player choosingPlayer, BoardChoices choices) =>
                    {
                        game.Board.GetInsanityTrack().AddToValue(-1);
                        handleMomentOfInsight(game, choosingPlayer, choices);
                    }));
        }

        protected override void addSummationEventsAndChoices()
        {
            SummationEvents.Add(
                new CardEffectPair(
                    doNothingChoice,
                    (Game game, Player choosingPlayer, BoardChoices choices) =>
                    {
                        findAspectTracksWithProp(game, Property.English, Property.Protestant).ForEach(t => t.AddToValue(2));
                    }));
        }
    }
}
