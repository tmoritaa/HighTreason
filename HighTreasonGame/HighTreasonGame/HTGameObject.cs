using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame
{
    public abstract class HTGameObject
    {
        public HashSet<Property> properties;

        protected Game game;

        public HTGameObject(Game _game, params Property[] _properties)
        {
            game = _game;
            properties = new HashSet<Property>(_properties);

            game.AddHTGameObject(this);
        }

        public virtual void RemoveChildrenHTGameObjects()
        {}
    }
}
