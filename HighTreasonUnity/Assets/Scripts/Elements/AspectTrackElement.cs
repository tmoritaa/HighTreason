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

    protected override void init()
    {
        base.init();
        aspectTrack = (AspectTrack)track;
    }

    protected override void updateUI()
    {
        base.updateUI();

        numUsedText.text = aspectTrack.TimesAffectedByAction.ToString();
    }
}
