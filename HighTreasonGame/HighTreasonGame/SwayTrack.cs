using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighTreasonGame
{
    public class SwayTrack : Track
    {
        public SwayTrack(int min, int max, int _gameId, params Property[] _properties)
            : base(0, min, max, _gameId, _properties)
        {
            properties.Add(Property.Sway);
        }
    }
}
