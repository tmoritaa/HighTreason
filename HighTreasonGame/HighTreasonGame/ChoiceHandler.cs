using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame
{
    public abstract class ChoiceHandler
    {
        public Player.PlayerType PlayerType
        {
            get; protected set;
        }

        public ChoiceHandler(Player.PlayerType playerType)
        {
            PlayerType = playerType;
        }

        public abstract void ChooseCardAndUsage(List<CardTemplate> cards, Game game, out Player.CardUsageParams outCardUsage);
        public abstract bool ChooseActionUsage(List<Track> choices, int actionPts, Jury deliberationJury, Game game, out Dictionary<Track, int> outTracks);
        public abstract bool ChooseAspectTracks(List<HTGameObject> choices, int numChoices, Game game, out List<AspectTrack> outAspectTracks);
        public abstract bool ChooseJuryAspects(List<List<HTGameObject>> choicesList, List<int> numChoicesList, Game game, out List<Jury.JuryAspect> outJuryAspects);
        public abstract bool ChooseJury(List<Jury> juries, Game game, out Jury outJury);
        public abstract bool ChooseMomentOfInsightUse(Game game, out BoardChoices.MomentOfInsightInfo outMoIInfo);
    }
}
