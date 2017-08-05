using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighTreasonGame
{
    public class TestChoiceHandler : IChoiceHandler
    {
        public void ChooseCardUsage()
        {
            Console.WriteLine("ChooseCardUsage called");
        }

        public List<Jury.JuryAspect> ChooseJuryAspects(List<HTGameObject> choices, int numChoices)
        {
            List<Jury.JuryAspect> juryAspects = new List<Jury.JuryAspect>();

            for (int i = 0; i < numChoices; ++i)
            {
                juryAspects.Add((Jury.JuryAspect)choices[i]);
            }

            return juryAspects;
        }

        public List<AspectTrack> ChooseAspectTracks(List<HTGameObject> choices, int numChoices)
        {
            List<AspectTrack> tracks = new List<AspectTrack>();

            for (int i = 0; i < numChoices; ++i)
            {
                tracks.Add((AspectTrack)choices[i]);
            }

            return tracks;
        }

        public string ChooseMomentOfInsightUse()
        {
            Console.WriteLine("Choose moment of insight use");
            return null;
        }
    }
}
