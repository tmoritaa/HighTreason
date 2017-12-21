using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame
{
    public class EvidenceTrack : Track
    {
        private Property[] uniqueProps;

        public EvidenceTrack(Game _game, params Property[] _properties) 
            : base(0, 0, 4, _game, _properties)
        {
            uniqueProps = _properties;
            Properties.Add(Property.Evidence);
        }

        // Copy constructor
        public EvidenceTrack(EvidenceTrack track, Game game)
            : base(track.Value, track.MinValue, track.MaxValue, game, track.Properties.ToArray())
        {
            List<Property> props = new List<Property>(track.Properties);
            props.Remove(Property.Track);
            props.Remove(Property.Evidence);
            uniqueProps = props.ToArray();
        }

        public override string ToString()
        {
            string str = "";

            foreach (Property prop in uniqueProps)
            {
                str += prop + " ";
            }

            str += " Evidence Track";
            return str;
        }
    }
}
