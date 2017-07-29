using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighTreasonGame
{
    public class EvidenceTrack : Track
    {
        public EvidenceTrack(HashSet<string> initProperties) 
            : base(0, 0, 4, initProperties)
        { }
    }
}
