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

            // Copy constructor
            public MomentOfInsightInfo(MomentOfInsightInfo moiInfo, Game game)
            {
                Use = moiInfo.Use;

                if (moiInfo.SummationCard != null)
                {
                    SummationCard = game.FindCard(moiInfo.SummationCard);
                }

                if (moiInfo.HandCard != null)
                {
                    HandCard = game.FindCard(moiInfo.HandCard);
                }
            }

            public bool CheckCloneEquality(MomentOfInsightInfo moiInfo)
            {
                bool equal = true;

                equal &= !object.ReferenceEquals(this, moiInfo);
                equal &= Use == moiInfo.Use;

                if (SummationCard != null)
                {
                    equal &= SummationCard.CheckCloneEquality(moiInfo.SummationCard);
                }
                else
                {
                    equal &= SummationCard == moiInfo.SummationCard;
                }
                
                if (HandCard != null)
                {
                    equal &= HandCard == moiInfo.HandCard;
                }

                return equal;
            }
        }

        public class CardPlayInfo
        {
            public int eventIdx = -1;
            public BoardChoices resultBoardChoice = null;

            public CardPlayInfo()
            {}

            // Copy constructor
            public CardPlayInfo(CardPlayInfo playInfo, Game game)
            {
                eventIdx = playInfo.eventIdx;

                if (playInfo.resultBoardChoice != null)
                {
                    resultBoardChoice = new BoardChoices(playInfo.resultBoardChoice, game);
                }
            }

            public bool CheckCloneEquality(CardPlayInfo playInfo)
            {
                bool equal = true;

                equal &= !object.ReferenceEquals(this, playInfo);
                equal &= eventIdx == playInfo.eventIdx;

                if (resultBoardChoice != null)
                {
                    equal &= resultBoardChoice.CheckCloneEquality(playInfo.resultBoardChoice);
                }
                else
                {
                    equal &= resultBoardChoice == playInfo.resultBoardChoice;
                }

                return equal;
            }
        }

        public Dictionary<BoardObject, int> SelectedObjs = new Dictionary<BoardObject, int>();
        public Dictionary<Card, int> SelectedCards = new Dictionary<Card, int>();
        public MomentOfInsightInfo MoIInfo = new MomentOfInsightInfo();
        public CardPlayInfo PlayInfo = new CardPlayInfo();

        public bool NotCancelled = true;

        public BoardChoices()
        {}

        // Copy constructor
        public BoardChoices(BoardChoices choices, Game game)
        {
            NotCancelled = choices.NotCancelled;

            foreach (var kv in choices.SelectedObjs)
            {
                List<BoardObject> bos = game.FindBO(
                    bo =>
                    {
                        return bo.Properties.SetEquals(kv.Key.Properties) && bo.ToString().Equals(kv.Key.ToString());
                    });

                System.Diagnostics.Debug.Assert(bos.Count == 1, "Cloning BoardChoices ended up not finding only one bo with identical Properties. Should never happen.");

                SelectedObjs.Add(bos.First(), kv.Value);
            }

            foreach (var kv in choices.SelectedCards)
            {
                SelectedCards.Add(game.FindCard(kv.Key), kv.Value);
            }

            PlayInfo = new CardPlayInfo(choices.PlayInfo, game);
            MoIInfo = new MomentOfInsightInfo(choices.MoIInfo, game);
        }

        public bool CheckCloneEquality(BoardChoices boardChoices)
        {
            bool equal = true;

            equal &= !object.ReferenceEquals(this, boardChoices);
            equal &= NotCancelled == boardChoices.NotCancelled;

            if (!equal)
            {
                Console.WriteLine("BoardChoices references were not equal");
                return equal;
            }

            foreach (BoardObject bo in SelectedObjs.Keys)
            {
                BoardObject cmpBo = boardChoices.SelectedObjs.Keys.ToList().Find(o => o.Properties.SetEquals(bo.Properties) && o.ToString().Equals(bo.ToString()));
                equal &= bo.CheckCloneEquality(cmpBo);
            }

            if (!equal)
            {
                Console.WriteLine("BoardChoices SelectedObjs were not equal");

                return equal;
            }

            foreach (Card card in SelectedCards.Keys)
            {
                Card cmpCard = boardChoices.SelectedCards.Keys.ToList().Find(c => c.Template.Name.Equals(card.Template.Name));

                equal &= card.CheckCloneEquality(cmpCard);
            }

            if (!equal)
            {
                Console.WriteLine("BoardChoices SelectedCards were not equal");
                return equal;
            }

            equal &= PlayInfo.CheckCloneEquality(boardChoices.PlayInfo);
            if (!equal)
            {
                Console.WriteLine("BoardChoices PlayInfo was not equal");
                return equal;
            }

            equal &= MoIInfo.CheckCloneEquality(boardChoices.MoIInfo);
            if (!equal)
            {
                Console.WriteLine("BoardChoices MoiInfo was not equal");
                return equal;
            }

            return equal;
        }

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
