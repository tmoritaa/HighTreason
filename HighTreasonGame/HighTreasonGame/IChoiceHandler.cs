﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighTreasonGame
{
    public interface IChoiceHandler
    {
        void ChooseCardUsage();
        List<AspectTrack> ChooseAspectTracks(List<HTGameObject> choices, int numChoices);
        List<Jury.JuryAspect> ChooseJuryAspects(List<HTGameObject> choices, int numChoices);
        string ChooseMomentOfInsightUse();
    }
}
