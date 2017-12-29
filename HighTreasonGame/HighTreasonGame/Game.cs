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
        public delegate void PlayedCardEvent(PlayerActionParams usageParams);
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

        public bool GameEnd
        {
            get; private set;
        }

        public Game(ChoiceHandler[] playerChoiceHandlers, string cardInfoJson, GameState.GameStateType startState = GameState.GameStateType.JurySelection)
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

        // Copy constructor
        public Game(Game game, ChoiceHandler[] playerChoiceHandlers)
        {
            this.Board = new Board(game.Board, this);
            this.Deck = new DeckHolder(game.Deck);
            this.Discards = new DiscardHolder(game.Discards);
            this.StartState = game.StartState;
            this.OfficersRecalledPlayable = game.OfficersRecalledPlayable;
            this.GameEnd = game.GameEnd;

            foreach (Player p in game.players.Values)
            {
                ChoiceHandler choiceHandler = playerChoiceHandlers[(p.Side == Player.PlayerSide.Prosecution) ? 0 : 1];

                players[p.Side] = new Player(p, this, choiceHandler);
            }

            foreach (var kv in game.states)
            {
                switch (kv.Key)
                {
                    case GameState.GameStateType.JurySelection:
                        this.states[kv.Key] = new JurySelectionState((JurySelectionState)kv.Value, this);
                        break;
                    case GameState.GameStateType.JuryDismissal:
                        this.states[kv.Key] = new JuryDismissalState((JuryDismissalState)kv.Value, this);
                        break;
                    case GameState.GameStateType.TrialInChief:
                        this.states[kv.Key] = new TrialInChiefState((TrialInChiefState)kv.Value, this);
                        break;
                    case GameState.GameStateType.Summation:
                        this.states[kv.Key] = new SummationState((SummationState)kv.Value, this);
                        break;
                    case GameState.GameStateType.Deliberation:
                        this.states[kv.Key] = new DeliberationState((DeliberationState)kv.Value, this);
                        break;
                }
            }

            this.CurState = this.states[game.CurState.StateType];

            NotifyStateStart += logStartOfState;
            NotifyGameEnd += logEndOfGame;
        }

        public bool CheckCloneEquality(Game game)
        {
            bool equal = true;

            equal &= this.Board.CheckCloneEquality(game.Board);

            if (!equal)
            {
                Console.WriteLine("Board was not equal");
                return equal;
            }

            equal &= this.Deck.CheckCloneEquality(game.Deck);

            if (!equal)
            {
                Console.WriteLine("Deck was not equal");
                return equal;
            }

            equal &= this.Discards.CheckCloneEquality(game.Discards);

            if (!equal)
            {
                Console.WriteLine("Discards was not equal");
                return equal;
            }

            equal &= GetPlayerOfSide(Player.PlayerSide.Prosecution).CheckCloneEquality(game.GetPlayerOfSide(Player.PlayerSide.Prosecution));
            equal &= GetPlayerOfSide(Player.PlayerSide.Defense).CheckCloneEquality(game.GetPlayerOfSide(Player.PlayerSide.Defense));

            if (!equal)
            {
                Console.WriteLine("Players were not equal");
                return equal;
            }

            equal &= this.StartState.GetType() == game.StartState.GetType();
            equal &= this.OfficersRecalledPlayable == game.OfficersRecalledPlayable;
            equal &= this.GameEnd == game.GameEnd;

            foreach (var kv in states)
            {
                equal &= kv.Value.CheckCloneEquality(game.states[kv.Key]);
                if (!equal)
                {
                    Console.WriteLine("State " + kv.Value.StateType + " was not equal");
                    return equal;
                }
            }

            equal &= this.CurState.StateType == game.CurState.StateType;

            return equal;
        }

        public Card FindCard(Card card)
        {
            Card retCard = null;

            switch (card.CardHolder.Id)
            {
                case CardHolder.HolderId.Deck:
                    retCard = Deck.Cards.Find(c => c.Template.Name.Equals(card.Template.Name));
                    break;
                case CardHolder.HolderId.Discard:
                    retCard = Discards.Cards.Find(c => c.Template.Name.Equals(card.Template.Name));
                    break;
                case CardHolder.HolderId.Summation:
                    {
                        Player player = this.GetPlayerOfSide(((PlayerCardHolder)card.CardHolder).Owner.Side);
                        retCard = player.SummationDeck.Cards.Find(c => c.Template.Name.Equals(card.Template.Name));
                    } break;
                case CardHolder.HolderId.Hand:
                    {
                        Player player = this.GetPlayerOfSide(((PlayerCardHolder)card.CardHolder).Owner.Side);
                        retCard = player.Hand.Cards.Find(c => c.Template.Name.Equals(card.Template.Name));
                    } break;
            }

            System.Diagnostics.Debug.Assert(retCard != null, "FindCard should never return null.");

            return retCard;
        }

        public HTAction Start()
        {
            SetNextState(StartState);
            return CurState.Start();
        }

        public HTAction Continue(object result)
        {
            return CurState.Continue(result);
        }

        public void SetNextState(GameState.GameStateType stateType)
        {
            CurState = states[stateType];
            CurState.InitState();
        }

        public void SignifyEndGame()
        {
            GameEnd = true;
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
            states.Add(GameState.GameStateType.Deliberation, new DeliberationState(this));
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
