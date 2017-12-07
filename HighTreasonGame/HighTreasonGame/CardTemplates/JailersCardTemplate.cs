using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json.Linq;

namespace HighTreasonGame.CardTemplates
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
                    (Game game, BoardChoices choices) =>
                    {
                        game.GetInsanityTrack().AddToValue(-1);
                    }));

            TrialEvents.Add(
                new CardEffectPair(
                    (Game game, Player choosingPlayer, ChoiceHandler choiceHandler) =>
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
                            choosingPlayer,
                            this.CardInfo.TrialInChiefInfos[0].Description,
                            out boardChoice);

                        return boardChoice;
                    },
                    (Game game, BoardChoices choices) =>
                    {
                        int numCardsToMul = choices.SelectedCards.Count;
                        choices.SelectedCards.Keys.ToList().ForEach(c => game.Discards.MoveCard(c));
                        game.Deck.DealCards(numCardsToMul).ForEach(c => game.CurPlayer.Hand.MoveCard(c));
                    }));
        }

        protected override void addSummationEventsAndChoices()
        {
            SummationEvents.Add(
                new CardEffectPair(
                    doNothingChoice,
                    doNothingEffect,
                    (Game game) =>
                    {
                        return false;
                    }));
        }
    }
}
