using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighTreasonGame
{
    public class Game
    {
        private static ConcurrentDictionary<int, Game> gameInstances = new ConcurrentDictionary<int, Game>();

        public Board board;

        private static object syncLock = new object();

        private List<HTGameObject> htGameObjects = new List<HTGameObject>();

        private int gameId;

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

            board = new Board(gameId);
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
            string outStr = board.ToString();

            return outStr;
        }

        
    }
}
