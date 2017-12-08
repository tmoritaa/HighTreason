using System;
using System.Collections.Generic;
using System.Linq;

using HighTreasonGame.GameStates;

namespace HighTreasonGame
{
    public class Player
    {
        public enum PlayerType
        {
            Human,
            AI,
        }

        public enum PlayerSide
        {
            Prosecution,
            Defense
        }

        public PlayerSide Side 
        {
            get; private set;
        }

        public PlayerType ChoiceType
        {
            get
            {
                return ChoiceHandler.PlayerType;
            }
        }

        public HandHolder Hand
        {
            get; private set;
        }
        
        public SummationDeckHolder SummationDeck
        {
            get; private set;
        }

        public bool PerformedMulligan
        {
            get; private set;
        }

        public ChoiceHandler ChoiceHandler
        {
            get; private set;
        }

        private Game game;

        public Player(PlayerSide _side, ChoiceHandler _choiceHandler, Game _game)
        {
            game = _game;
            Side = _side;
            ChoiceHandler = _choiceHandler;
            SummationDeck = new SummationDeckHolder();
            Hand = new HandHolder();
            PerformedMulligan = false;
        }

        public void AddHandToSummation()
        {
            Hand.MoveAllCardsToHolder(SummationDeck);
        }

        public void RevealCardInSummation()
        {
            SummationDeck.RevealRandomCardInSummation();
        }

        public void PerformMulligan()
        {
            int prevHandSize = Hand.Cards.Count;

            Hand.MoveAllCardsToHolder(game.Discards);
            Hand.SetupHand(game.Deck.DealCards(prevHandSize - 1));

            PerformedMulligan = true;
        }

        public override string ToString()
        {
            string outStr = Side + " player:\n";

            outStr += "Hand = \n";
            foreach (Card card in Hand.Cards)
            {
                outStr += card.Template.Name + "\n";
            }

            outStr += "Summation = \n";
            foreach (Card card in SummationDeck.Cards)
            {
                outStr += card.Template.Name + "\n";
            }

            return outStr;
        }
    }
}
