using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json.Linq;

namespace HighTreasonGame.CardTemplates
{
    public class JailersCardTemplate : CardTemplate
    {
        public JailersCardTemplate(JObject json)
            : base("Jailers", 1, json)
        { }

        protected override void addSelectionEventsAndChoices()
        {
            SelectionEvents.Add(
                new CardEffectPair(
                    genRevealOrPeakCardChoice(new HashSet<Property>() {}, 1, false, this.CardInfo.JurySelectionPairs[0].Description),
                    peekAllAspects));
        }

        protected override void addTrialEventsAndChoices()
        {
            TrialEvents.Add(
                new CardEffectPair(
                    doNothingChoice,
                    (Game game, BoardChoices choices) =>
                    {
                        game.GetInsanityTrack().AddToValue(-1);
                    }));

            TrialEvents.Add(
                new CardEffectPair(
                    (Game game, ChoiceHandler choiceHandler) =>
                    {
                        List<Card> selectableCards = game.CurPlayer.Hand.Cards;

                        BoardChoices boardChoice;
                        choiceHandler.ChooseCards(
                            selectableCards,
                            (Dictionary<Card, int> selected) =>
                            {
                                bool noDup = true;

                                foreach (var val in selected.Values) {
                                    if (val != 1)
                                    {
                                        noDup = false;
                                        break;
                                    }
                                }

                                return noDup;
                            },
                            (List<Card> remainingChoices, Dictionary<Card, int> selected) =>
                            {
                                foreach (var card in selected.Keys)
                                {
                                    remainingChoices.Remove(card);
                                }

                                return remainingChoices;
                            },
                            (Dictionary<Card, int> selected, bool isDone) =>
                            {
                                return (isDone && selected.Count == 1) || selected.Count == 2;
                            },
                            true,
                            game,
                            this.CardInfo.TrialInChiefPairs[0].Description,
                            out boardChoice);

                        return boardChoice;
                    },
                    (Game game, BoardChoices choices) =>
                    {
                        int numCardsToMul = choices.selectedCards.Count;
                        choices.selectedCards.Keys.ToList().ForEach(c => game.Discards.MoveCard(c));
                        game.Deck.DealCards(numCardsToMul).ForEach(c => game.CurPlayer.Hand.MoveCard(c));
                    }));
        }

        protected override void addSummationEventsAndChoices()
        {
            SummationEvents.Add(
                new CardEffectPair(
                    doNothingChoice,
                    doNothingEffect,
                    false));
        }
    }
}
