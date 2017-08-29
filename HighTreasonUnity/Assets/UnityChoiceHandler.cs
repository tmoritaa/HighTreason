using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HighTreasonGame;

public class UnityChoiceHandler : ChoiceHandler
{
    public UnityChoiceHandler()
        : base(Player.PlayerType.Human)
    {}


    public override bool ChooseActionUsage(List<Track> choices, int actionPts, Jury deliberationJury, Game game, out Dictionary<Track, int> outTracks)
    {
        throw new NotImplementedException();
    }

    public override bool ChooseAspectTracks(List<HTGameObject> choices, int numChoices, Game game, out List<AspectTrack> outAspectTracks)
    {
        throw new NotImplementedException();
    }

    public override void ChooseCardAndUsage(List<CardTemplate> cards, Game game, out Player.CardUsageParams outCardUsage)
    {
        throw new NotImplementedException();
    }

    public override bool ChooseJury(List<Jury> juries, Game game, out Jury outJury)
    {
        throw new NotImplementedException();
    }

    public override bool ChooseJuryAspects(List<List<HTGameObject>> choicesList, List<int> numChoicesList, Game game, out List<Jury.JuryAspect> outJuryAspects)
    {
        throw new NotImplementedException();
    }

    public override bool ChooseMomentOfInsightUse(Game game, out BoardChoices.MomentOfInsightInfo outMoIInfo)
    {
        throw new NotImplementedException();
    }
}