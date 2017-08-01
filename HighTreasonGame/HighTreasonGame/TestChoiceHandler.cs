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

        public AspectTrack ChooseAspectTrack(List<HTGameObject> choices)
        {
            foreach (HTGameObject gameObject in choices)
            {
                Console.WriteLine(gameObject);
            }

            return (AspectTrack)choices[0];
        }

        public Jury.JuryAspect ChooseJuryAspects(List<HTGameObject> choices)
        {
            Console.WriteLine("ChooseJuryAspects called");

            return null;
        }
    }
}
