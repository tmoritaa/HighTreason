using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame
{
    public class SwayTrack : Track
    {
        public SwayTrack(int min, int max, Game game, params Property[] _properties)
            : base(0, min, max, game, _properties)
        {
            properties.Add(Property.Sway);
        }
    }
}
