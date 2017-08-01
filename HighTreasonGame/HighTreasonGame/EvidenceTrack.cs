using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighTreasonGame
{
    public class EvidenceTrack : Track
    {
        public EvidenceTrack(int _gameId, params Property[] _properties) 
            : base(0, 0, 4, _gameId, _properties)
        {
            properties.Add(Property.Evidence);
        }
    }
}
