using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HighTreasonGame.GameStates;

namespace HighTreasonGame
{
    public class Game
    {
        public delegate void StateStartEvent();
        public delegate void StartOfTurnEvent();
        public delegate void PlayedCardEvent(Player.CardUsageParams usageParams);
        public delegate void GameEndEvent(Player.PlayerSide winningPlayerSide);

        public StateStartEvent NotifyStateStart;
        public StartOfTurnEvent NotifyStartOfTurn;
        public PlayedCardEvent NotifyPlayedCard;
        public GameEndEvent NotifyGameEnd;

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

        private List<BoardObject> boardObjects = new List<BoardObject>();

        private Dictionary<Player.PlayerSide, Player> players = new Dictionary<Player.PlayerSide, Player>();

        private Dictionary<Type, GameState> states = new Dictionary<Type, GameState>();
        public GameState CurState
        {
            get; private set;
        }

        public Game(ChoiceHandler[] playerChoiceHandlers)
        {
            Board = new Board(this);
            Deck = new Deck(this);
            Discards = new List<CardTemplate>();

            int idx = 0;
            foreach (Player.PlayerSide side in new Player.PlayerSide[] { Player.PlayerSide.Prosecution, Player.PlayerSide.Defense })
            {
                players.Add(side, new Player(side, playerChoiceHandlers[idx], this));
                idx += 1;
            }

            CurPlayer = players[0];

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
            RemoveBoardObject(jury);
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

        public void AddBoardObject(BoardObject bo)
        {
            boardObjects.Add(bo);
        }

        public void RemoveBoardObject(BoardObject bo)
        {
            bo.RemoveChildrenBoardObjects();
            boardObjects.Remove(bo);
        }

        public List<BoardObject> FindBO(Func<BoardObject, bool> condition)
        {
            return boardObjects.FindAll(htgo => condition(htgo));
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
