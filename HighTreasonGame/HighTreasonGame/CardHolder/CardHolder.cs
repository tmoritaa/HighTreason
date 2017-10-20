using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame
{
    public abstract class CardHolder
    {
        public enum HolderId
        {
            Discard,
            Deck,
            Summation,
            Hand,
        }

        public HolderId Id
        {
            get; protected set;
        }

        public List<Card> Cards
        {
            get; protected set;
        }

        public CardHolder(HolderId id)
        {
            Id = id;
            Cards = new List<Card>();
        }

        public void MoveCard(Card card)
        {
            if (card.CardHolder != null)
            {
                card.CardHolder.RemoveCard(card);
            }
            AddCard(card);
        }

        public void MoveAllCardsToHolder(CardHolder holder)
        {
            for (int i = Cards.Count - 1; i >= 0; --i)
            {
                Card card = Cards[i];
                holder.MoveCard(card);
            }
        }

        protected virtual void AddCard(Card card)
        {
            card.CardHolder = this;
            Cards.Add(card);
        }

        protected virtual void RemoveCard(Card card)
        {
            card.CardHolder = null;
            Cards.Remove(card);
        }
    }
}
