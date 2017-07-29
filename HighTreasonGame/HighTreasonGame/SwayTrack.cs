using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighTreasonGame
{
    public class SwayTrack : Track
    {
        public SwayTrack(int min, int max, HashSet<string> _properties)
            : base(0, min, max, new HashSet<string>() { GameConstants.PROP_SWAY })
        {
            foreach (string property in _properties)
            {
                properties.Add(property);
            }
        }
    }
}
