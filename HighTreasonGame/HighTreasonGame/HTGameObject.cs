using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighTreasonGame
{
    public abstract class HTGameObject
    {
        public HashSet<Property> properties;

        protected int gameId;

        public HTGameObject(int _gameId, params Property[] _properties)
        {
            gameId = _gameId;
            properties = new HashSet<Property>(_properties);

            Game.GetGameFromId(gameId).AddHTGameObject(this);
        }

        public virtual void RemoveChildrenHTGameObjects()
        {}
    }
}
