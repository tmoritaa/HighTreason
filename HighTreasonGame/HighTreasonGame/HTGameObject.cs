using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame
{
    public abstract class HTGameObject
    {
        public HashSet<Property> Properties
        {
            get; private set;
        }

        protected Game game;

        public HTGameObject(Game _game, params Property[] _properties)
        {
            game = _game;
            Properties = new HashSet<Property>(_properties);

            game.AddHTGameObject(this);
        }

        public virtual void RemoveChildrenHTGameObjects()
        {}
    }
}
