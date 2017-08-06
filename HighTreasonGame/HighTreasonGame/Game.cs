using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        private static ConcurrentDictionary<int, Game> gameInstances = new ConcurrentDictionary<int, Game>();

        private static object syncLock = new object();
        private List<HTGameObject> htGameObjects = new List<HTGameObject>();

        private int gameId;

        private Dictionary<Player.PlayerSide, Player> players = new Dictionary<Player.PlayerSide, Player>();

        private Dictionary<GameState.StateType, GameState> states = new Dictionary<GameState.StateType, GameState>();
        private GameState curState;

        public static Game GetGameFromId(int id)
        {
            return gameInstances[id];
        }

        public Game()
        {
            lock (syncLock)
            {
                gameId = gameInstances.Count;
            }
            gameInstances.TryAdd(gameId, this);

            Board = new Board(gameId);
            Deck = new Deck(gameId);
            Discards = new List<CardTemplate>();

            foreach (Player.PlayerSide side in new Player.PlayerSide[] { Player.PlayerSide.Prosecution, Player.PlayerSide.Defense })
            {
                players.Add(side, new Player(side, new TestChoiceHandler(), gameId));
            }

            initStates();
        }

        public void StartGame()
        {
            GotoStateAndStart(GameState.StateType.JurySelection);
        }

        public void GotoStateAndStart(GameState.StateType stateType)
        {
            curState = states[stateType];
            curState.StartState();
        }

        public void PassToNextPlayer()
        {
            switch (CurPlayer.Side)
            {
                case Player.PlayerSide.Prosecution:
                    CurPlayer = players[Player.PlayerSide.Defense];
                    break;
                case Player.PlayerSide.Defense:
                    CurPlayer = players[Player.PlayerSide.Prosecution];
                    break;
            }
        }

        public void ShuffleDiscardBackToDeck()
        {
            Deck.AddCardsToDeck(Discards);
            Discards.Clear();
        }

        public List<Player> GetPlayers()
        {
            return players.Values.ToList();
        }

        public Player GetPlayerOfSide(Player.PlayerSide side)
        {
            return players[side];
        }


        public void AddHTGameObject(HTGameObject go)
        {
            htGameObjects.Add(go);
        }

        public List<HTGameObject> GetHTGOFromCondition(Func<HTGameObject, bool> condition)
        {
            return htGameObjects.FindAll(htgo => condition(htgo));
        }

        public override string ToString()
        {
            string outStr = Board.ToString();

            return outStr;
        }

        private void initStates()
        {
            states.Add(GameState.StateType.JurySelection, new JurySelectionState(gameId));
        }
    }
}
