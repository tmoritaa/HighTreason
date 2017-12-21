using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame
{
    public abstract class BoardObject
    {
        public HashSet<Property> Properties
        {
            get; private set;
        }

        protected Game game;

        public BoardObject(Game _game, params Property[] _properties)
        {
            game = _game;
            Properties = new HashSet<Property>(_properties);

            game.AddBoardObject(this);
        }

        public virtual bool CheckCloneEquality(BoardObject bo)
        {
            bool equal = true;

            equal &= !object.ReferenceEquals(this, bo);
            equal &= Properties.SetEquals(bo.Properties);

            return equal;
        }

        public virtual void RemoveChildrenBoardObjects()
        {}
    }
}
