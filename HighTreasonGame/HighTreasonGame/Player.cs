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
            SummationDeck = new SummationDeckHolder(this);
            Hand = new HandHolder(this);
            PerformedMulligan = false;
        }

        // Copy constructor
        public Player(Player player, Game _game, ChoiceHandler choiceHandler)
        {
            game = _game;
            Side = player.Side;
            ChoiceHandler = choiceHandler;
            SummationDeck = new SummationDeckHolder(player.SummationDeck, this);
            Hand = new HandHolder(player.Hand, this);
            PerformedMulligan = player.PerformedMulligan;
        }

        public bool CheckCloneEquality(Player player)
        {
            bool equal = true;

            equal &= !object.ReferenceEquals(game, player.game);
            equal &= !object.ReferenceEquals(this, player);
            equal &= Side == player.Side;

            equal &= SummationDeck.CheckCloneEquality(player.SummationDeck);
            equal &= Hand.CheckCloneEquality(player.Hand);

            equal &= PerformedMulligan == player.PerformedMulligan;

            return equal;
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
            return Side.ToString();
        }
    }
}
