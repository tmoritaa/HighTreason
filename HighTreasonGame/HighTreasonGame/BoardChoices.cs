﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame
{
    public class BoardChoices
    {
        public class MomentOfInsightInfo
        {
            public enum MomentOfInsightUse
            {
                Swap,
                Reveal,
                NotChosen
            }

            public MomentOfInsightUse Use;
            public Card SummationCard;
            public Card HandCard;
        }

        public class CardPlayInfo
        {
            public int eventIdx = -1;
            public BoardChoices resultBoardChoice;
        }

        public Dictionary<BoardObject, int> SelectedObjs = new Dictionary<BoardObject, int>();
        public Dictionary<Card, int> SelectedCards = new Dictionary<Card, int>();
        public MomentOfInsightInfo MoIInfo = new MomentOfInsightInfo();
        public CardPlayInfo PlayInfo = new CardPlayInfo();

        public bool NotCancelled = true;
    }
}
