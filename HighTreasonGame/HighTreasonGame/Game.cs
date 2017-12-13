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
        public delegate void PlayedCardEvent(ChoiceHandler.PlayerActionParams usageParams);
        public delegate void GameEndEvent(Player.PlayerSide winningPlayerSide, bool winByNotEnoughGuilt, int finalScore);

        public StateStartEvent NotifyStateStart;
        public StartOfTurnEvent NotifyStartOfTurn;
        public PlayedCardEvent NotifyPlayerActionPerformed;
        public GameEndEvent NotifyGameEnd;

        public Board Board 
        {
            get; private set;
        }

        public DeckHolder Deck
        {
            get; private set;
        }

        public DiscardHolder Discards
        {
            get; private set;
        }

        public GameState.GameStateType StartState
        {
            get; private set;
        }

        private List<BoardObject> boardObjects = new List<BoardObject>();

        private Dictionary<Player.PlayerSide, Player> players = new Dictionary<Player.PlayerSide, Player>();

        private Dictionary<GameState.GameStateType, GameState> states = new Dictionary<GameState.GameStateType, GameState>();
        public GameState CurState
        {
            get; private set;
        }

        public bool OfficersRecalledPlayable = false;

        private bool gameEnd = false;

        public Game(ChoiceHandler[] playerChoiceHandlers, string cardInfoJson, GameState.GameStateType startState=GameState.GameStateType.JurySelection)
        {
            StartState = startState;

            Board = new Board(this);

            CardTemplateGenerator tempGenerator = new CardTemplateGenerator(cardInfoJson);

            List<Card> cards = new List<Card>();
            tempGenerator.GetAllCardTemplates().ForEach(c => cards.Add(new Card(c)));
            Deck = new DeckHolder(cards);

            Discards = new DiscardHolder();

            int idx = 0;
            foreach (Player.PlayerSide side in new Player.PlayerSide[] { Player.PlayerSide.Prosecution, Player.PlayerSide.Defense })
            {
                players.Add(side, new Player(side, playerChoiceHandlers[idx], this));
                idx += 1;
            }

            NotifyStateStart += logStartOfState;
            NotifyGameEnd += logEndOfGame;

            initStates();
        }

        public void StartGame()
        {
            SetNextState(StartState);

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

        public List<Player> GetPlayers()
        {
            return players.Values.ToList();
        }

        public Player GetPlayerOfSide(Player.PlayerSide side)
        {
            return players[side];
        }

        public Player GetOtherPlayer(Player curPlayer)
        {
            Player.PlayerSide oppositeSide = (curPlayer.Side == Player.PlayerSide.Defense) ? Player.PlayerSide.Prosecution : Player.PlayerSide.Defense;
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

        private void logStartOfState()
        {
            FileLogger.Instance.Log("Started " + CurState.StateType + " state");
        }

        private void logEndOfGame(Player.PlayerSide winningPlayerSide, bool winByNotEnoughGuilt, int finalScore)
        {
            string str = winningPlayerSide + " won with " + (winByNotEnoughGuilt ? "not enough guilt" : finalScore + " points");
            FileLogger.Instance.Log(str);
        }
    }
}
