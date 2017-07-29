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

        public AspectTrack(int initValue, HashSet<string> _properties) 
            : base(initValue, 0, 10, _properties)
        {
            properties.Add(GameConstants.PROP_SWAY);
            swayTrack = new SwayTrack(0, 3, properties);
        }

        public override string ToString()
        {
            string outStr = base.ToString() + "\n";

            outStr += "\t" + swayTrack.ToString();

            return outStr;
        }

    }
}
