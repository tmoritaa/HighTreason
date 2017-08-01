using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighTreasonGame
{
    public class Jury : HTGameObject
    {
        public class JuryAspect
        {
            private bool revealed = false;
            private Property property;

            public JuryAspect(Property _property)
            {
                property = _property;
            }

            public override string ToString()
            {
                return property + " revealed?=" + revealed;
            }
        }

        private SwayTrack track;
        private int actionPoints;
        private Dictionary<Property, JuryAspect> aspects = new Dictionary<Property, JuryAspect>();

        public Jury(int swayMax, int _actionPoints, int _gameId, Property religionAspect, Property languageAspect, Property occupationAspect)
            : base(_gameId, Property.Jury, Property.Religion, Property.Language, Property.Occupation)
        {
            actionPoints = _actionPoints;

            track = new SwayTrack(-swayMax, swayMax, _gameId, Property.Jury);

            aspects.Add(Property.Religion, new JuryAspect(religionAspect));
            aspects.Add(Property.Language, new JuryAspect(languageAspect));
            aspects.Add(Property.Occupation, new JuryAspect(occupationAspect));
        }

        public override string ToString()
        {
            string outStr = string.Empty;
            foreach(JuryAspect aspect in aspects.Values)
            {
                outStr += aspect + "\n";
            }

            outStr += track.ToString() + "\n";
            outStr += "Action Points=" + actionPoints;

            return outStr;
        }
    }
}
