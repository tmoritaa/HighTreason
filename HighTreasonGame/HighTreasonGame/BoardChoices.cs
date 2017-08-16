using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame
{
    public class BoardChoices
    {
        public class MomentOfInsightInfo
        {
            public enum MomentOfInsightUse
            {
                Swap,
                Reveal,
                NotChosen
            }

            public MomentOfInsightUse Use;
            public CardTemplate SummationCard;
            public CardTemplate HandCard;
        }

        public List<AspectTrack> AspectTracks = new List<AspectTrack>();
        public List<Jury.JuryAspect> JuryAspects = new List<Jury.JuryAspect>();
        public MomentOfInsightInfo MoIInfo = new MomentOfInsightInfo();

        public bool NotCancelled = true;
    }
}
