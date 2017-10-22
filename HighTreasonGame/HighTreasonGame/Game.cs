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
        public delegate void GameEndEvent(Player.PlayerSide winningPlayerSide, bool winByNotEnoughGuilt, int finalScore);

        public StateStartEvent NotifyStateStart;
        public StartOfTurnEvent NotifyStartOfTurn;
        public PlayedCardEvent NotifyPlayedCard;
        public GameEndEvent NotifyGameEnd;

        public Board Board 
        {
            get; private set;
        }

        public DeckHolder Deck
        {
            get; private set;
        }

        public Player CurPlayer {
            get; set;
        }
        
        public DiscardHolder Discards
        {
            get; private set;
        }

        private Dictionary<Type, List<BoardObject>> boardObjects = new Dictionary<Type, List<BoardObject>>();

        private Dictionary<Player.PlayerSide, Player> players = new Dictionary<Player.PlayerSide, Player>();

        private Dictionary<GameState.GameStateType, GameState> states = new Dictionary<GameState.GameStateType, GameState>();
        public GameState CurState
        {
            get; private set;
        }

        private bool gameEnd = false;

        public Game(ChoiceHandler[] playerChoiceHandlers)
        {
            Board = new Board(this);

            List<Card> cards = new List<Card>();
            CardTemplateManager.Instance.GetAllCardTemplates().ForEach(c => cards.Add(new Card(this, c)));
            Deck = new DeckHolder(cards);

            Discards = new DiscardHolder();

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
            SetNextState(GameState.GameStateType.JurySelection);

            while (!gameEnd)
            {
                CurState.StartState();
            }
        }

        public void SetNextState(GameState.GameStateType stateType)
        {
            CurState = states[stateType];
        }

        public void SignifyEndGame()
        {
            gameEnd = true;
        }

        public void PassToNextPlayer()
        {
            CurPlayer = GetOtherPlayer();
        }

        public void ShuffleDiscardBackToDeck()
        {
            Discards.MoveAllCardsToHolder(Deck);
            Deck.Shuffle();
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
            if (!boardObjects.ContainsKey(bo.GetType()))
            {
                boardObjects.Add(bo.GetType(), new List<BoardObject>());
            }
            boardObjects[bo.GetType()].Add(bo);
        }

        public void RemoveBoardObject(BoardObject bo)
        {
            bo.RemoveChildrenBoardObjects();
            boardObjects[bo.GetType()].Remove(bo);
        }

        public List<BoardObject> FindBO(Func<Type, bool> typeFilterCond, Func<BoardObject, bool> condition)
        {
            List<BoardObject> searchBos = new List<BoardObject>();

            foreach (Type t in boardObjects.Keys)
            {
                if (typeFilterCond(t))
                {
                    searchBos.AddRange(boardObjects[t]);
                }
            }

            return searchBos.FindAll(htgo => condition(htgo));
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
            foreach (Card card in Discards.Cards)
            {
                outStr += card.Template.Name + "\n";
            }

            outStr += Board.ToString();

            return outStr;
        }

        private void initStates()
        {
            states.Add(GameState.GameStateType.JurySelection, new JurySelectionState(this));
            states.Add(GameState.GameStateType.JuryDismissal, new JuryDismissalState(this));
            states.Add(GameState.GameStateType.TrialInChief, new TrialInChiefState(this));
            states.Add(GameState.GameStateType.Summation, new SummationState(this));
            states.Add(GameState.GameStateType.Deliberation, new DelibrationState(this));
        }
    }
}
