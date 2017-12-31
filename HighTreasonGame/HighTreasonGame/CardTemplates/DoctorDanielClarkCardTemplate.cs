using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame
{
    [CardTemplateAttribute]
    public class DoctorDanielClarkCardTemplate : CardTemplate
    {
        public DoctorDanielClarkCardTemplate()
            : base("Doctor Daniel Clark", 3, Player.PlayerSide.Defense)
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
                    genAspectTrackForModCardChoice(new HashSet<Property>() { Property.Religion }, 1, -2, false, this.CardInfo.TrialInChiefInfos[0].Description),
                    (Game game, Player choosingPlayer, BoardChoices boardChoices) =>
                    {
                        game.Board.GetInsanityTrack().AddToValue(1);
                        findAspectTracksWithProp(game, Property.English).ForEach(t => t.AddToValue(-2));

                        if (boardChoices.SelectedObjs.Count > 0)
                        {
                            ((AspectTrack)boardChoices.SelectedObjs.Keys.First()).AddToValue(-2);
                        }
                    }));
        }

        protected override void addSummationEventsAndChoices()
        {
            SummationEvents.Add(
                new CardEffectPair(
                    doNothingChoice,
                    (Game game, Player choosingPlayer, BoardChoices boardChoices) =>
                    {
                        game.Board.GetInsanityTrack().AddToValue(1);
                    }));

            SummationEvents.Add(
                new CardEffectPair(
                    doNothingChoice,
                    (Game game, Player choosingPlayer, BoardChoices boardChoices) =>
                    {
                        foreach (Jury jury in game.Board.Juries)
                        {
                            if (!jury.SwayTrack.IsLocked)
                            {
                                jury.SwayTrack.AddToValue(-2);
                            }
                        }
                    }));
        }
    }
}
