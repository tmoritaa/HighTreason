using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using HighTreasonGame;

public class AspectTrackElement : TrackElement 
{
    [SerializeField]
    private Text numUsedText;

    private AspectTrack aspectTrack;

    public override void InitUIElement()
    {
        base.InitUIElement();

        aspectTrack = (AspectTrack)track;
    }

    protected override void updateUIElement()
    {
        base.updateUIElement();

        numUsedText.text = aspectTrack.TimesAffectedByAction.ToString();
    }
}
