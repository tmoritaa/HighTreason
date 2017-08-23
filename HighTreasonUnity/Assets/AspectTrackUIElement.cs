using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using HighTreasonGame;

public class AspectTrackUIElement : TrackUIElement 
{
    private AspectTrack aspectTrack;

    [SerializeField]
    Text numUsedText;

    protected override void Awake()
    {
        base.Awake();

        aspectTrack = (AspectTrack)track;
    }

    protected override void Update()
    {
        base.Update();

        numUsedText.text = aspectTrack.TimesAffectedByAction.ToString();
    }


}
