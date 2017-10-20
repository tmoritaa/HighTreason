using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame
{
    public class DeckHolder : CardHolder
    {
        public DeckHolder(List<Card> _cards) : base(HolderId.Deck)
        {
            _cards.ForEach(c => MoveCard(c));

            Shuffle();
        }

        public List<Card> DealCards(int numCards)
        {
            List<Card> retCards = Cards.Take(numCards).ToList();
            Cards = Cards.Except(retCards).ToList();

            return retCards;
        }

        public void Shuffle()
        {
            int n = Cards.Count;
            while (n > 1)
            {
                n--;
                int k = GlobalRandom.GetRandomNumber(0, n + 1);
                Card value = Cards[k];
                Cards[k] = Cards[n];
                Cards[n] = value;
            }
        }
    }
}
