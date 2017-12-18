using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json.Linq;

namespace HighTreasonGame
{
    [CardTemplateAttribute]
    public class TwoCursesCardTemplate : CardTemplate
    {
        public TwoCursesCardTemplate()
            : base("\"Two Curses\"", 2, Player.PlayerSide.Prosecution)
        { }

        protected override void addSelectionEventsAndChoices()
        {
            SelectionEvents.Add(
                new CardEffectPair(
                    genRevealOrPeakCardChoice(new HashSet<Property> { Property.Language }, 3, true, this.CardInfo.JurySelectionInfos[0].Description),
                    revealAllAspects));

            SelectionEvents.Add(
                new CardEffectPair(
                    genRevealOrPeakCardChoice(new HashSet<Property> { Property.Occupation }, 2, true, this.CardInfo.JurySelectionInfos[1].Description),
                    revealAllAspects));
        }

        protected override void addTrialEventsAndChoices()
        {
            TrialEvents.Add(
                new CardEffectPair(
                    (Game game, Player choosingPlayer) =>
                    {
                        return handleMomentOfInsightChoice(new Player.PlayerSide[] { Player.PlayerSide.Prosecution, Player.PlayerSide.Defense },
                                game, choosingPlayer);
                    },
                    (Game game, Player choosingPlayer, BoardChoices choices) =>
                    {
                        foreach (Jury jury in game.Board.Juries)
                        {
                            jury.Aspects[2].Peek(choosingPlayer.Side);
                        }

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
                        findAspectTracksWithProp(game, Property.Merchant, Property.GovWorker, Property.Farmer).ForEach(t => t.AddToValue(t.Properties.Contains(Property.Farmer) ? -2 : 1));
                    }));
        }
    }
}
