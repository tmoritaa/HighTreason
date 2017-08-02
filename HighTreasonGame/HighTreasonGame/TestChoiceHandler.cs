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

        public Jury.JuryAspect ChooseJuryAspects(List<HTGameObject> choices)
        {
            Console.WriteLine("ChooseJuryAspects called");

            return null;
        }

        public List<AspectTrack> ChooseAspectTracks(List<HTGameObject> choices, int numChoices)
        {
            foreach (HTGameObject gameObject in choices)
            {
                Console.WriteLine(gameObject);
            }

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
