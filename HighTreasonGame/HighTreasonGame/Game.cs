﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HighTreasonGame.GameStates;
using HighTreasonGame.ChoiceHandlers;

namespace HighTreasonGame
{
    public class Game
    {
        public Board Board 
        {
            get; private set;
        }

        public Deck Deck
        {
            get; private set;
        }

        public Player CurPlayer {
            get; set;
        }
        
        public List<CardTemplate> Discards
        {
            get; private set;
        }

        private List<HTGameObject> htGameObjects = new List<HTGameObject>();

        private Dictionary<Player.PlayerSide, Player> players = new Dictionary<Player.PlayerSide, Player>();

        private Dictionary<Type, GameState> states = new Dictionary<Type, GameState>();
        public GameState CurState
        {
            get; private set;
        }

        public EventHandler EventHandler
        {
            get; private set;
        }

        public Game(EventHandler _eventHandler)
        {
            EventHandler = _eventHandler;

            Board = new Board(this);
            Deck = new Deck(this);
            Discards = new List<CardTemplate>();

            foreach (Player.PlayerSide side in new Player.PlayerSide[] { Player.PlayerSide.Prosecution, Player.PlayerSide.Defense })
            {
                players.Add(side, new Player(side, new ConsolePlayerChoiceHandler(), this));
            }

            initStates();
        }

        public void StartGame()
        {
            setNextState(typeof(JurySelectionState));

            while (true)
            {
                CurState.StartState();
            }
        }

        public void setNextState(Type stateType)
        {
            EventHandler.GotoNextState(stateType);

            CurState = states[stateType];
        }

        public void PassToNextPlayer()
        {
            CurPlayer = GetOtherPlayer();
        }

        public void ShuffleDiscardBackToDeck()
        {
            Deck.AddCardsToDeck(Discards);
            Discards.Clear();
        }

        public void RemoveJury(Jury jury)
        {
            Board.Juries.Remove(jury);
            RemoveHTGameObject(jury);
        }

        public List<Player> GetPlayers()
        {
            return players.Values.ToList();
        }

        public Player GetPlayerOfSide(Player.PlayerSide side)
        {
            return players[side];
        }

        public Player GetOtherPlayer()
        {
            Player.PlayerSide oppositeSide = (CurPlayer.Side == Player.PlayerSide.Defense) ? Player.PlayerSide.Prosecution : Player.PlayerSide.Defense;
            return players[oppositeSide];
        }

        public void AddHTGameObject(HTGameObject go)
        {
            htGameObjects.Add(go);
        }

        public void RemoveHTGameObject(HTGameObject go)
        {
            go.RemoveChildrenHTGameObjects();
            htGameObjects.Remove(go);
        }

        public List<HTGameObject> GetHTGOFromCondition(Func<HTGameObject, bool> condition)
        {
            return htGameObjects.FindAll(htgo => condition(htgo));
        }

        public override string ToString()
        {
            string outStr = string.Empty;

            outStr += "Players:\n";
            foreach (Player player in players.Values)
            {
                outStr += player;
            }

            outStr += "Discard:\n";
            foreach (CardTemplate card in Discards)
            {
                outStr += card.Name + "\n";
            }

            outStr += Board.ToString();

            return outStr;
        }

        private void initStates()
        {
            states.Add(typeof(JurySelectionState), new JurySelectionState(this));
            states.Add(typeof(JuryDismissalState), new JuryDismissalState(this));
            states.Add(typeof(TrialInChiefState), new TrialInChiefState(this));
            states.Add(typeof(SummationState), new SummationState(this));
            states.Add(typeof(DelibrationState), new DelibrationState(this));
        }
    }
}
