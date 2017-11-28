using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using HighTreasonGame;

public class SwayTrackElement : BoardObjectElement
{
    [SerializeField]
    private Text text;

    private SwayTrack swayTrack;

    public void InitSwayTrack(Jury jury)
    {
        swayTrack = jury.SwayTrack;
        SelectKey = swayTrack;
        updateUI();
    }

    protected override void init()
    {
        // Do nothing.
    }

    protected override void updateUI()
    {
        text.text = swayTrack.Value.ToString();
    }
}