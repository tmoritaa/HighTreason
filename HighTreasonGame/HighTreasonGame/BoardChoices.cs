using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame
{
    public class BoardChoices
    {
        public List<AspectTrack> aspectTracks = new List<AspectTrack>();
        public List<EvidenceTrack> evidenceTracks = new List<EvidenceTrack>();
        public List<Jury.JuryAspect> juryAspects = new List<Jury.JuryAspect>();
    }
}
