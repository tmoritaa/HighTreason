using System;
using System.Collections.Generic;

using HighTreasonGame.GameStates;

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
        
        public SummationDeck SummationDeck
        {
            get; private set;
        }

        public Player(PlayerSide _side, IChoiceHandler _choiceHandler, Game _game)
        {
            game = _game;
            Side = _side;
            choiceHandler = _choiceHandler;
            SummationDeck = new SummationDeck();
        }

        public void SetupHand(List<CardTemplate> _hand)
        {
            Hand = _hand;
        }

        public void PlayCard()
        {
            bool cardPlayed = false;
            while (!cardPlayed)
            {
                CardUsageParams cardUsage;
                if (!choiceHandler.ChooseCardAndUsage(Hand, game, out cardUsage))
                {
                    continue;
                }

                // Remove card being used from hand.
                Hand.Remove(cardUsage.card);

                if (cardUsage.usage == CardUsageParams.UsageType.Event)
                {
                    cardPlayed = cardUsage.card.PlayAsEvent(game, (int)cardUsage.misc[0], choiceHandler);
                }
                else if (cardUsage.usage == CardUsageParams.UsageType.Action)
                {
                    cardPlayed = cardUsage.card.PlayAsAction(game, choiceHandler);
                }

                if (cardPlayed)
                {
                    game.EventHandler.PlayedCard(this, cardUsage);

                    // Move used card to discard.
                    game.Discards.Add(cardUsage.card);
                }
                else
                {
                    // Readd card to hand since undoing selection.
                    Hand.Add(cardUsage.card);
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

        public void AddHandToSummation()
        {
            Hand.ForEach(c => SummationDeck.AddCard(c));
            Hand.Clear();
        }

        public void RevealCardInSummation()
        {
            CardTemplate revealedCard = SummationDeck.RevealRandomCardInSummation();
            Console.WriteLine(revealedCard.Name + " revealed in summation");
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
            foreach (CardTemplate card in SummationDeck.AllCards)
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
