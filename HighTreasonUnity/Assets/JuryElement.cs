using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using HighTreasonGame;

public class JuryElement : GameUIElement 
{
    [SerializeField]
    private int id;

    [SerializeField]
    private Text actionPtText;

    [SerializeField]
    private Text swayText;

    [SerializeField]
    private Text aspectReligionText;

    [SerializeField]
    private Text aspectLanguageText;

    [SerializeField]
    private Text aspectOccupationText;

    private Jury jury;

    public override void InitUIElement()
    {
        jury = (Jury)GameManager.Instance.Game.Board.Juries.Find(j => j.Id == id);
        updateUIElement();
        base.InitUIElement();
    }

    protected override void updateUIElement()
    {
        actionPtText.text = jury.ActionPoints + " Delibration Action Points";
        swayText.text = jury.SwayTrack.Value.ToString();
        aspectReligionText.text = determineAspectStr(jury.Aspects[0]);
        aspectLanguageText.text = determineAspectStr(jury.Aspects[1]);
        aspectOccupationText.text = determineAspectStr(jury.Aspects[2]);
    }

    protected string determineAspectStr(Jury.JuryAspect aspect)
    {
        if (aspect.IsVisibleToPlayer(GameManager.Instance.Game.CurPlayer.Side))
        {
            string ret = aspect.Aspect.ToString()[0].ToString();

            if (aspect.IsPeeked)
            {
                ret += "?";
            }

            return ret;
        }
        else
        {
            return "?";
        }
    }
}
