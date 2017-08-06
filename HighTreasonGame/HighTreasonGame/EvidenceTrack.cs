using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighTreasonGame
{
    public class EvidenceTrack : Track
    {
        public EvidenceTrack(Game _game, params Property[] _properties) 
            : base(0, 0, 4, _game, _properties)
        {
            properties.Add(Property.Evidence);
        }
    }
}
