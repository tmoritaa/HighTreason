using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace HighTreasonGame
{
    public class HandHolder : PlayerCardHolder
    {
        public List<Card> SelectableCards
        {
            get
            {
                return Cards.FindAll(c => !c.BeingPlayed);
            }
        }

        public HandHolder(Player player) : base(HolderId.Hand, player)
        {}

        // Copy constructor
        public HandHolder(HandHolder holder, Player player) : base(holder, player)
        {}

        public void SetupHand(List<Card> cards)
        {
            Debug.Assert(Cards.Count == 0, "HandHolder SetupHand called when hand size was not zero.");

            cards.ForEach(c => MoveCard(c));
        }
    }
}
