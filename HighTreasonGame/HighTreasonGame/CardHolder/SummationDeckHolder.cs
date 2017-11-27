using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame
{
    public class SummationDeckHolder : CardHolder
    {
        private List<bool> hiddenStatus = new List<bool>();

        public SummationDeckHolder() : base(HolderId.Summation)
        {
            for (int i = 0; i < Cards.Count; ++i)
            {
                hiddenStatus.Add(false);
            }
        }

        protected override void AddCard(Card card)
        {
            hiddenStatus.Add(true);
            base.AddCard(card);
        }

        protected override void RemoveCard(Card card)
        {
            int idx = Cards.FindIndex(c => c == card);
            hiddenStatus.RemoveAt(idx);
            base.RemoveCard(card);
        }

        public void RevealRandomCardInSummation()
        {
            List<Card> hiddenCards = new List<Card>();

            for (int i = 0; i < Cards.Count; ++i)
            {
                if (hiddenStatus[i])
                {
                    hiddenCards.Add(Cards[i]);
                }
            }

            int randIdx = GlobalRandom.GetRandomNumber(0, hiddenCards.Count);
            Card card = hiddenCards[randIdx];

            card.Revealed = true;
        }
    }
}
