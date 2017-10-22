using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace HighTreasonGame
{
    public class HandHolder : CardHolder
    {
        public List<Card> SelectableCards
        {
            get
            {
                return Cards.FindAll(c => !c.BeingPlayed);
            }
        }

        public HandHolder() : base(HolderId.Hand)
        {}

        public void SetupHand(List<Card> cards)
        {
            Debug.Assert(cards.Count == 0, "HandHolder SetupHand called when hand size was not zero.");

            cards.ForEach(c => MoveCard(c));
        }
    }
}
