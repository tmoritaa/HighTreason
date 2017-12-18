using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json.Linq;

namespace HighTreasonGame
{
    /*
    [CardTemplateAttribute]
    public class ReverendCharlesPitbladoCardTemplate : CardTemplate
    {
        public ReverendCharlesPitbladoCardTemplate()
            : base("Reverend Charles Pitblado", 2, Player.PlayerSide.Prosecution)
        { }

        protected override void addSelectionEventsAndChoices()
        {
            SelectionEvents.Add(
                new CardEffectPair(
                    genRevealOrPeakCardChoice(new HashSet<Property> { Property.Language }, 2, true, this.CardInfo.JurySelectionInfos[0].Description),
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
                    doNothingChoice,
                    (Game game, Player choosingPlayer, BoardChoices choices) =>
                    {
                        game.Board.GetInsanityTrack().AddToValue(-1);
                    }));

            TrialEvents.Add(
                new CardEffectPair(
                    (Game game, Player choosingPlayer, ChoiceHandler choiceHandler) =>
                    {
                        CardChoice peekFunc = genRevealOrPeakCardChoice(new HashSet<Property>() { Property.Religion }, 2, false, this.CardInfo.TrialInChiefInfos[1].Description);
                        BoardChoices boardChoices = peekFunc(game, choosingPlayer, choiceHandler);

                        if (boardChoices.NotCancelled)
                        {
                            boardChoices.NotCancelled = handleMomentOfInsightChoice(new Player.PlayerSide[] { Player.PlayerSide.Prosecution },
                                game, choosingPlayer, choiceHandler, out boardChoices.MoIInfo);
                        }

                        return boardChoices;
                    },
                    (Game game, Player choosingPlayer, BoardChoices choices) =>
                    {
                        peekAllAspects(game, choosingPlayer, choices);
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
    }*/
}
