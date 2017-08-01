using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighTreasonGame
{
    public class HTGameObject
    {
        protected Game game;
        protected HashSet<string> properties;

        public HTGameObject(Game _game, HashSet<string> _properties)
        {
            game = _game;
            properties = new HashSet<string>(_properties);
        }
    }
}
