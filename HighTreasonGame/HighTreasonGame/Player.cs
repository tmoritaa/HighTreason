using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        List<CardTemplate> hand = new List<CardTemplate>();
        List<CardTemplate> cardsForSummation = new List<CardTemplate>();

        public Player(PlayerSide _side, IChoiceHandler _choiceHandler, Game _game)
        {
            game = _game;
            Side = _side;
            choiceHandler = _choiceHandler;
        }

        public void SetupHand(List<CardTemplate> _hand)
        {
            hand = _hand;
        }

        public void PlayCard(Type curStateType)
        {
            CardUsageParams cardUsage = choiceHandler.ChooseCardAndUsage(hand);

            if (cardUsage.usage == CardUsageParams.UsageType.Event)
            {
                int idx = (int)cardUsage.misc[0];

                cardUsage.card.PlayAsEvent(curStateType, game, idx, choiceHandler);

                System.Console.WriteLine("Player " + Side + " played " + cardUsage.card.Name + " as event at idx " + idx);
            }
            else if (cardUsage.usage == CardUsageParams.UsageType.Action)
            {
                // TODO: implement.
            }

            discardCard(cardUsage.card);
        }

        public void DismissJury()
        {
            Jury jury = choiceHandler.ChooseJuryToDismiss(game.Board.Juries);

            Console.WriteLine("Dismissed Jury\n" + jury);

            game.RemoveJury(jury);
        }

        public bool MustPass()
        {
            return hand.Count <= 2;
        }

        public void AddHandToSummation()
        {
            cardsForSummation.AddRange(hand);
            hand.Clear();
        }

        public override string ToString()
        {
            string outStr = "Player:\n";

            outStr += "Side = " + Side + "\n";

            outStr += "Hand = \n";
            foreach (CardTemplate card in hand)
            {
                outStr += card.Name + "\n";
            }

            outStr += "Summation = \n";
            foreach (CardTemplate card in cardsForSummation)
            {
                outStr += card.Name + "\n";
            }

            return outStr;
        }

        private void discardCard(CardTemplate card)
        {
            hand.Remove(card);
            game.Discards.Add(card);
        }
    }
}
