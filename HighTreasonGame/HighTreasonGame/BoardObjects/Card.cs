using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame
{
    public class Card : BoardObject
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

        public Card(Game _game, CardTemplate _cardTemplate) : base(_game, Property.Card)
        {
            Template = _cardTemplate;
        }
    }
}
