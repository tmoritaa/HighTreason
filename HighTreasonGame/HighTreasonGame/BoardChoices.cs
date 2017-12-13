using System;
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

            public MomentOfInsightInfo()
            {
                Use = MomentOfInsightUse.NotChosen;
            }
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

        // Note that this function assumes that a card and a board object cannot be selected at the same time, which is currently true.
        public string ToStringForEvent()
        {
            string str = "";

            if (SelectedObjs.Count > 0)
            {
                foreach(var kv in SelectedObjs)
                {
                    str += kv.Key + " selected " + kv.Value + " times\n";
                }
            }
            else if (SelectedCards.Count > 0)
            {
                foreach(var kv in SelectedCards)
                {
                    str += kv.Key + " selected " + kv.Value + " times\n";
                }
            }

            if (MoIInfo.Use != MomentOfInsightInfo.MomentOfInsightUse.NotChosen)
            {
                if (MoIInfo.Use == MomentOfInsightInfo.MomentOfInsightUse.Reveal)
                {
                    str += "Reveal selected for MoI\n";
                }
                else if (MoIInfo.Use == MomentOfInsightInfo.MomentOfInsightUse.Swap)
                {
                    str += "Swap selected for MoI. Hand=" + MoIInfo.HandCard + " Sum=" + MoIInfo.SummationCard + "\n";
                }
            }

            if (PlayInfo.eventIdx >= 0)
            {
                str += "event idx=" + PlayInfo.eventIdx + " selected with choices\n";
                str += PlayInfo.resultBoardChoice.ToStringForEvent();
            }

            return str;
        }
    }
}
