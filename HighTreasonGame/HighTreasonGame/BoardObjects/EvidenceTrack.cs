using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame
{
    public class EvidenceTrack : Track
    {
        private Property[] uniqueProp;

        public EvidenceTrack(Game _game, params Property[] _properties) 
            : base(0, 0, 4, _game, _properties)
        {
            uniqueProp = _properties;
            Properties.Add(Property.Evidence);
        }

        public override string ToString()
        {
            string str = "";

            foreach (Property prop in uniqueProp)
            {
                str += prop + " ";
            }

            str += " Evidence Track";
            return str;
        }
    }
}
