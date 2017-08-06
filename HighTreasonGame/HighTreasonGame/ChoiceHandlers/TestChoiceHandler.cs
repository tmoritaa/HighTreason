using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighTreasonGame.ChoiceHandlers
{
    public class TestChoiceHandler : IChoiceHandler
    {
        public Player.CardUsageParams ChooseCardAndUsage(List<CardTemplate> cards)
        {
            Player.CardUsageParams cardUsage = new Player.CardUsageParams();
            cardUsage.card = cards[0];
            cardUsage.usage = Player.CardUsageParams.UsageType.Event;
            cardUsage.misc.Add(0);

            return cardUsage;
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
