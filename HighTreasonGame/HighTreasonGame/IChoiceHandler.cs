using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame
{
    public interface IChoiceHandler
    {
        void ChooseCardAndUsage(List<CardTemplate> cards, Game game, out Player.CardUsageParams outCardUsage);
        bool ChooseActionUsage(List<Track> choices, int actionPts, Game game, out Dictionary<Track, int> outTracks);
        bool ChooseAspectTracks(List<HTGameObject> choices, int numChoices, Game game, out List<AspectTrack> outAspectTracks);
        bool ChooseJuryAspects(List<List<HTGameObject>> choicesList, List<int> numChoicesList, Game game, out List<Jury.JuryAspect> outJuryAspects);
        bool ChooseJuryToDismiss(List<Jury> juries, Game game, out List<Jury> outJury);
        bool ChooseMomentOfInsightUse(Game game, out BoardChoices.MomentOfInsightInfo outMoIInfo);
        void ChooseJuryForDelibration(List<Jury> juries, Game game, out Jury outJury);
    }
}
