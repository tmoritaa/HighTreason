using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighTreasonGame
{
    public class Jury
    {
        private class JuryAspect
        {
            private bool revealed = false;
            private string property;

            public JuryAspect(string _property)
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
        private Dictionary<string, JuryAspect> aspects = new Dictionary<string, JuryAspect>();

        public Jury(int swayMax, int _actionPoints, string religionAspect, string languageAspect, string occupationAspect)
        {
            actionPoints = _actionPoints;

            track = new SwayTrack(-swayMax, swayMax, new HashSet<string>() { GameConstants.PROP_JURY });

            aspects.Add(GameConstants.PROP_RELIGION, new JuryAspect(religionAspect));
            aspects.Add(GameConstants.PROP_LANGUAGE, new JuryAspect(languageAspect));
            aspects.Add(GameConstants.PROP_OCCUPATION, new JuryAspect(occupationAspect));
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
