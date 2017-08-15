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

        public IEventHandler EventHandler
        {
            get; private set;
        }

        public Game(IEventHandler _eventHandler)
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
            SetNextState(typeof(JurySelectionState));

            while (CurState != null)
            {
                CurState.StartState();
            }
        }

        public void SetNextState(Type stateType)
        {
            EventHandler.GotoNextState(stateType);

            CurState = states[stateType];
        }

        public void SignifyEndGame()
        {
            CurState = null;
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

        public bool ValidateActionUsage(Dictionary<Track, int> affectedTracks, int actionPts, Jury deliberationJury)
        {
            bool isDeliberation = deliberationJury != null;

            bool isValid = true;
            int actionPtsLeft = actionPts;
            foreach (Track track in affectedTracks.Keys)
            {
                int numTimesAffected = affectedTracks[track];

                int affectLimit = 2;

                if (isDeliberation && track.Properties.Contains(Property.Sway) && track.Properties.Contains(Property.Jury))
                {
                    affectLimit = 0;
                    foreach (Jury.JuryAspect aspect in deliberationJury.Aspects)
                    {
                        affectLimit += (track.Properties.Contains(aspect.Aspect)) ? 1 : 0;
                    }
                }

                if (numTimesAffected > affectLimit)
                {
                    isValid = false;
                    break;
                }

                if (track.GetType() == typeof(SwayTrack))
                {
                    actionPtsLeft -= numTimesAffected;
                }
                else if (track.GetType() == typeof(AspectTrack))
                {
                    actionPtsLeft -= (numTimesAffected > 1) ? 3 : 1;
                }
                else
                {
                    System.Diagnostics.Debug.Assert(false, "Track chosen to affect by action is not sway or aspect track. Should never happen!");
                }
            }

            isValid &= (actionPtsLeft == 0);

            return isValid;
        }

        public Player.PlayerSide DetermineWinner()
        {
            int totalScore = 0;
            foreach (Jury jury in Board.Juries)
            {
                int score = jury.CalculateGuiltScore();
                totalScore += score;
                // TODO: debug log. Remove later.
                Console.WriteLine("Jury " + jury.Id + " contributed " + score + " guilt score");
            }

            // TODO: debug log. Remove later.
            Console.WriteLine("Total guilt score is " + totalScore);

            Player.PlayerSide winningPlayer = totalScore >= GameConstants.PROSECUTION_SCORE_THRESHOLD ? Player.PlayerSide.Prosecution : Player.PlayerSide.Defense;
            return winningPlayer;
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

        public EvidenceTrack GetInsanityTrack()
        {
            return Board.EvidenceTracks.Find(t => t.Properties.Contains(Property.Insanity));
        }

        public EvidenceTrack GetGuiltTrack()
        {
            return Board.EvidenceTracks.Find(t => t.Properties.Contains(Property.Guilt));
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
