using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighTreasonGame
{
    public class SwayTrack : Track
    {
        public SwayTrack(Game _game, HashSet<string> _properties, int min, int max)
            : base(_game, _properties, 0, min, max)
        {
            properties.Add(GameConstants.PROP_SWAY);
        }
    }
}
