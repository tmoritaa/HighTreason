using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame
{
    public class SummationDeck
    {
        public List<CardTemplate> HiddenCards
        {
            get; private set;
        }
        public List<CardTemplate> RevealedCards
        {
            get; private set;
        }

        public List<CardTemplate> AllCards
        {
            get
            {
                List<CardTemplate> cards = new List<CardTemplate>();
                cards.AddRange(HiddenCards);
                cards.AddRange(RevealedCards);
                return cards;
            }
        }

        public SummationDeck()
        {
            HiddenCards = new List<CardTemplate>();
            RevealedCards = new List<CardTemplate>();
        }

        public List<CardTemplate> DealoutCards()
        {
            List<CardTemplate> cards = AllCards;

            HiddenCards.Clear();
            RevealedCards.Clear();

            return cards;
        }

        public void AddCard(CardTemplate card)
        {
            HiddenCards.Add(card);
        }

        public void RemoveCard(CardTemplate card)
        {
            HiddenCards.Remove(card);
            RevealedCards.Remove(card);
        }

        public CardTemplate RevealRandomCardInSummation()
        {
            int randIdx = GlobalRandom.GetRandomNumber(0, HiddenCards.Count);
            CardTemplate card = HiddenCards[randIdx];
            HiddenCards.Remove(card);
            RevealedCards.Add(card);

            return card;
        }
    }
}
