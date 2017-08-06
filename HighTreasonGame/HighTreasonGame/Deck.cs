using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighTreasonGame
{
    public class Deck
    {
        private int gameId;

        private List<CardTemplate> cards;

        public Deck(int _gameId)
        {
            gameId = _gameId;

            cards = CardTemplateManager.Instance.GetAllCards();

            Shuffle();
        }

        public List<CardTemplate> DealCards(int numCards)
        {
            List<CardTemplate> retCards = cards.Take(numCards).ToList();
            cards = cards.Except(retCards).ToList();

            return retCards;
        }

        public void Shuffle()
        {
            int n = cards.Count;
            while (n > 1)
            {
                n--;
                int k = GlobalRandom.GetRandomNumber(0, n + 1);
                CardTemplate value = cards[k];
                cards[k] = cards[n];
                cards[n] = value;
            }
        }

        public void AddCardsToDeck(List<CardTemplate> cards)
        {
            cards.AddRange(cards);
            Shuffle();
        }
    }
}
