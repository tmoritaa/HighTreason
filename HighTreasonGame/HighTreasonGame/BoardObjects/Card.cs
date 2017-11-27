using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame
{
    public class Card
    {
        public CardTemplate Template
        {
            get; private set;
        }

        public CardHolder CardHolder
        {
            get; set;
        }

        public bool CanBePlayed
        {
            get
            {
                return CardHolder.Id == CardHolder.HolderId.Hand;
            }
        }

        public bool BeingPlayed = false;

        public bool Revealed = false;

        public Card(CardTemplate _cardTemplate)
        {
            Template = _cardTemplate;
        }
    }
}
