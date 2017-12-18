using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json.Linq;

namespace HighTreasonGame
{
    [CardTemplateAttribute]
    public class MajorCrozierCardTemplate : CardTemplate
    {
        public MajorCrozierCardTemplate()
            : base("Major Crozier", 3, Player.PlayerSide.Prosecution)
        { }

        protected override void addSelectionEventsAndChoices()
        {
            SelectionEvents.Add(
                new CardEffectPair(
                    genRevealOrPeakCardChoice(new HashSet<Property> { Property.Language }, 3, true, this.CardInfo.JurySelectionInfos[0].Description),
                    revealAllAspects));

            SelectionEvents.Add(
                new CardEffectPair(
                    genRevealOrPeakCardChoice(new HashSet<Property> { Property.Religion }, 1, true, this.CardInfo.JurySelectionInfos[1].Description),
                    revealAllAspects));
        }

        protected override void addTrialEventsAndChoices()
        {
            TrialEvents.Add(
                new CardEffectPair(
                    (Game game, Player choosingPlayer) =>
                    {
                        return handleMomentOfInsightChoice(
                            new Player.PlayerSide[] { Player.PlayerSide.Prosecution, Player.PlayerSide.Defense }, 
                            game, 
                            choosingPlayer);
                    },
                    (Game game, Player choosingPlayer, BoardChoices choices) =>
                    {
                        findAspectTracksWithProp(game, Property.Farmer).ForEach(t => t.AddToValue(calcModValueBasedOnSide(2, choosingPlayer)));
                        handleMomentOfInsight(game, choosingPlayer, choices);
                    }));

            TrialEvents.Add(
                new CardEffectPair(
                    (Game game, Player choosingPlayer) =>
                    {
                        BoardChoices boardChoices = new BoardChoices();
                        return handleMomentOfInsightChoice(
                            new Player.PlayerSide[] { Player.PlayerSide.Defense },
                            game, 
                            choosingPlayer);
                    },
                    (Game game, Player choosingPlayer, BoardChoices choices) =>
                    {
                        findAspectTracksWithProp(game, Property.French).ForEach(t => t.AddToValue(-2));
                        handleMomentOfInsight(game, choosingPlayer, choices);
                    }));
        }

        protected override void addSummationEventsAndChoices()
        {
            SummationEvents.Add(
                new CardEffectPair(
                    genAspectTrackForModCardChoice(new HashSet<Property>(), 1, 1, true, this.CardInfo.SummationInfos[0].Description),
                    (Game game, Player choosingPlayer, BoardChoices choices) =>
                    {
                        int modVal = calcModValueBasedOnSide(1, choosingPlayer);
                        game.Board.GetGuiltTrack().AddToValue(modVal);
                        ((AspectTrack)choices.SelectedObjs.Keys.First()).AddToValue(modVal);
                    }));
        }
    }
}
