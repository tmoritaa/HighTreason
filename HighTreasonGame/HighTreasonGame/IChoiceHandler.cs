using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame
{
    public interface IChoiceHandler
    {
        Player.CardUsageParams ChooseCardAndUsage(List<CardTemplate> cards);
        List<AspectTrack> ChooseAspectTracks(List<HTGameObject> choices, int numChoices);
        List<Jury.JuryAspect> ChooseJuryAspects(List<HTGameObject> choices, int numChoices);
        Jury ChooseJuryToDismiss(List<Jury> juries);
        string ChooseMomentOfInsightUse();
    }
}
