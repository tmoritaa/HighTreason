using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighTreasonGame
{
    public class AspectTrack : Track
    {
        private const int MAX_TIMES_MODABLE = 3;

        private SwayTrack swayTrack;

        public AspectTrack(Game _game, HashSet<string> _properties, int initValue) 
            : base(_game, _properties, initValue, 0, 10)
        {
            properties.Add(GameConstants.PROP_SWAY);
            swayTrack = new SwayTrack(_game, properties, 0, 3);
        }

        public override string ToString()
        {
            string outStr = base.ToString() + "\n";

            outStr += "\t" + swayTrack.ToString();

            return outStr;
        }

    }
}
