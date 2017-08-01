using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighTreasonGame
{
    public class EvidenceTrack : Track
    {
        public EvidenceTrack(Game _game, HashSet<string> _properties) 
            : base(_game, _properties, 0, 0, 4)
        { }
    }
}
