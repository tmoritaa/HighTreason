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

        public AspectTrack(int _value, int _gameId, params Property[] _properties) 
            : base(_value, 0, 10, _gameId, _properties)
        {
            properties.Add(Property.Aspect);
            swayTrack = new SwayTrack(0, 3, _gameId, _properties);
        }

        public override string ToString()
        {
            string outStr = "-" + base.ToString() + "\n";

            outStr += "\t" + swayTrack.ToString();

            return outStr;
        }

    }
}
