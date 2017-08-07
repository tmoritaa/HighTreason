using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame
{
    public interface IChoiceHandler
    {
        Player.CardUsageParams ChooseCardAndUsage(List<CardTemplate> cards, Game game);
        List<AspectTrack> ChooseAspectTracks(List<HTGameObject> choices, int numChoices, Game game);
        List<Jury.JuryAspect> ChooseJuryAspects(List<HTGameObject> choices, int numChoices, Game game);
        Jury ChooseJuryToDismiss(List<Jury> juries, Game game);
        string ChooseMomentOfInsightUse(Game game);
    }
}
