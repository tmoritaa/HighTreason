using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json.Linq;

namespace HighTreasonGame
{
    [CardTemplateAttribute]
    public class JailersCardTemplate : CardTemplate
    {
        public JailersCardTemplate()
            : base("Jailers", 1, Player.PlayerSide.Prosecution)
        { }

        protected override void addSelectionEventsAndChoices()
        {
            SelectionEvents.Add(
                new CardEffectPair(
                    genRevealOrPeakCardChoice(new HashSet<Property>() {}, 1, false, this.CardInfo.JurySelectionInfos[0].Description),
                    peekAllAspects));
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
                    (Game game, Player choosingPlayer) =>
                    {
                        List<Card> selectableCards = choosingPlayer.Hand.Cards;

                        return new HTAction(
                            ChoiceHandler.ChoiceType.Cards,
                            choosingPlayer.ChoiceHandler,
                            selectableCards,
                            (Func<Dictionary<Card, int>, bool>)
                                ((Dictionary<Card, int> selected) =>
                                {
                                    bool noDup = true;

                                    foreach (var val in selected.Values)
                                    {
                                        if (val != 1)
                                        {
                                            noDup = false;
                                            break;
                                        }
                                    }

                                    return noDup;
                                }),
                            (Func<List<Card>, Dictionary<Card, int>, List<Card>>)
                                ((List<Card> remainingChoices, Dictionary<Card, int> selected) =>
                                {
                                    foreach (var card in selected.Keys)
                                    {
                                        remainingChoices.Remove(card);
                                    }

                                    return remainingChoices;
                                }),
                            (Func<Dictionary<Card, int>, bool, bool>)
                                ((Dictionary<Card, int> selected, bool isDone) =>
                                {
                                    return (isDone && selected.Count == 1) || selected.Count == 2;
                                }),
                            true,
                            game,
                            choosingPlayer,
                            this.CardInfo.TrialInChiefInfos[0].Description);
                    },
                    (Game game, Player choosingPlayer, BoardChoices choices) =>
                    {
                        int numCardsToMul = choices.SelectedCards.Count;
                        choices.SelectedCards.Keys.ToList().ForEach(c => game.Discards.MoveCard(c));
                        game.Deck.DealCards(numCardsToMul).ForEach(c => choosingPlayer.Hand.MoveCard(c));
                    }));
        }

        protected override void addSummationEventsAndChoices()
        {
            SummationEvents.Add(
                new CardEffectPair(
                    doNothingChoice,
                    doNothingEffect,
                    (Game game, Player choosingPlayer) =>
                    {
                        return false;
                    }));
        }
    }
}
