using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame
{
    public class Player
    {
        public enum PlayerSide
        {
            Prosecution,
            Defense
        }

        public class CardUsageParams
        {
            public enum UsageType
            {
                Event,
                Action,
            }

            public CardTemplate card;
            public UsageType usage;
            public List<object> misc = new List<object>();
        }

        public PlayerSide Side 
        {
            get; private set;
        }

        private Game game;
        private IChoiceHandler choiceHandler;

        public List<CardTemplate> Hand
        {
            get; private set;
        }

        public List<CardTemplate> CardsForSummation
        {
            get; private set;
        }

        public Player(PlayerSide _side, IChoiceHandler _choiceHandler, Game _game)
        {
            game = _game;
            Side = _side;
            choiceHandler = _choiceHandler;
            CardsForSummation = new List<CardTemplate>();
        }

        public void SetupHand(List<CardTemplate> _hand)
        {
            Hand = _hand;
        }

        public void PlayCard(Type curStateType)
        {
            bool cardPlayed = false;
            while (!cardPlayed)
            {
                CardUsageParams cardUsage;
                if (!choiceHandler.ChooseCardAndUsage(Hand, game, out cardUsage))
                {
                    continue;
                }

                if (cardUsage.usage == CardUsageParams.UsageType.Event)
                {
                    cardPlayed = cardUsage.card.PlayAsEvent(curStateType, game, (int)cardUsage.misc[0], choiceHandler);
                }
                else if (cardUsage.usage == CardUsageParams.UsageType.Action)
                {
                    // TODO: implement.
                }

                if (cardPlayed)
                {
                    game.EventHandler.PlayedCard(this, cardUsage);

                    // TODO: need a way to not show card in hand when chosen, but still not completely discard it to keep card position.
                    // Currently will bug with MoI swap if played card is selected for swap.
                    discardCard(cardUsage.card);
                }
            }
        }

        public void DismissJury()
        {
            List<Jury> jury;
            while (!choiceHandler.ChooseJuryToDismiss(game.Board.Juries, game, out jury))
            {};

            Console.WriteLine("Dismissed Jury\n" + jury[0]);

            game.RemoveJury(jury[0]);
        }

        public bool MustPass()
        {
            return Hand.Count <= 2;
        }

        public void AddHandToSummation()
        {
            CardsForSummation.AddRange(Hand);
            Hand.Clear();
        }

        public void RevealCardInSummation()
        {
            int randIdx = GlobalRandom.GetRandomNumber(0, CardsForSummation.Count);
            // TODO: add persistence to shown card.
            Console.WriteLine(CardsForSummation[randIdx].Name);
        }

        public override string ToString()
        {
            string outStr = Side + " player:\n";

            outStr += "Hand = \n";
            foreach (CardTemplate card in Hand)
            {
                outStr += card.Name + "\n";
            }

            outStr += "Summation = \n";
            foreach (CardTemplate card in CardsForSummation)
            {
                outStr += card.Name + "\n";
            }

            return outStr;
        }

        private void discardCard(CardTemplate card)
        {
            Hand.Remove(card);
            game.Discards.Add(card);
        }
    }
}
