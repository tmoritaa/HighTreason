using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame.ChoiceHandlers
{
    public class TestChoiceHandler : IChoiceHandler
    {
        public Player.CardUsageParams ChooseCardAndUsage(List<CardTemplate> cards, Game game)
        {
            Player.CardUsageParams cardUsage = new Player.CardUsageParams();
            cardUsage.card = cards[0];
            cardUsage.usage = Player.CardUsageParams.UsageType.Event;
            cardUsage.misc.Add(0);

            return cardUsage;
        }

        public List<Jury.JuryAspect> ChooseJuryAspects(List<HTGameObject> choices, int numChoices, Game game)
        {
            List<Jury.JuryAspect> juryAspects = new List<Jury.JuryAspect>();

            int uptoIdx = Math.Min(numChoices, choices.Count);
            for (int i = 0; i < uptoIdx; ++i)
            {
                juryAspects.Add((Jury.JuryAspect)choices[i]);
            }

            return juryAspects;
        }

        public List<AspectTrack> ChooseAspectTracks(List<HTGameObject> choices, int numChoices, Game game)
        {
            List<AspectTrack> tracks = new List<AspectTrack>();

            int uptoIdx = Math.Min(numChoices, tracks.Count);
            for (int i = 0; i < uptoIdx; ++i)
            {
                tracks.Add((AspectTrack)choices[i]);
            }

            return tracks;
        }

        public string ChooseMomentOfInsightUse(Game game)
        {
            Console.WriteLine("Choose moment of insight use");
            return null;
        }

        public Jury ChooseJuryToDismiss(List<Jury> juries, Game game)
        {
            return juries[0];
        }
    }
}
