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
    private int id;

    [SerializeField]
    private Text actionPtText;

    [SerializeField]
    private Text swayText;

    [SerializeField]
    private List<JuryAspectElement> ownedJuryAspectElements;
    
    private Jury jury;

    protected override void init()
    {
        jury = (Jury)GameManager.Instance.Game.Board.Juries.Find(j => j.Id == id);
        setupBOAndGO(jury);

        ownedJuryAspectElements.ForEach(e => e.InitJuryAspect(id));

        updateUI();
    }

    protected override void updateUI()
    {
        actionPtText.text = jury.ActionPoints + " Delibration Action Points";
        swayText.text = jury.SwayTrack.Value.ToString();
    }
}
