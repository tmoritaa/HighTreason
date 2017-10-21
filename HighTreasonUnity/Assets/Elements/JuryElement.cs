using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using HighTreasonGame;

public class JuryElement : BoardObjectElement 
{
    [SerializeField]
    private Text actionPtText;

    [SerializeField]
    private SwayTrackElement swayTrackElement;

    [SerializeField]
    private List<JuryAspectElement> ownedJuryAspectElements;

    public int Idx
    {
        get; set;
    }

    private Jury jury;

    protected override void init()
    {
        jury = (Jury)GameManager.Instance.Game.Board.Juries[Idx];
        setupBOAndGO(jury);

        ownedJuryAspectElements.ForEach(e => e.InitJuryAspect(jury));
        swayTrackElement.InitSwayTrack(jury);

        updateUI();
    }

    protected override void updateUI()
    {
        base.updateUI();

        actionPtText.text = jury.ActionPoints + " Delibration Action Points";
    }
}
