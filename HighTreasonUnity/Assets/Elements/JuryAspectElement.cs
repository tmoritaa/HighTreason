using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using HighTreasonGame;

public class JuryAspectElement : BoardObjectElement
{
    [SerializeField]
    private Property aspectProp;

    [SerializeField]
    private Text text;

    private Jury.JuryAspect juryAspect;

    public void InitJuryAspect(Jury jury)
    {
        juryAspect = jury.Aspects.Find(ja => ja.Properties.Contains(aspectProp));
        setupBOAndGO(juryAspect);
        updateUI();
    }

    protected override void init()
    {
        // Do nothing here. Wait until JuryElement calls other initialize function.
    }

    protected override void updateUI()
    {
        base.updateUI();

        string str = string.Empty;
        if (juryAspect.IsVisibleToPlayer(GameManager.Instance.Game.CurPlayer.Side))
        {
            str = juryAspect.Aspect.ToString().Substring(0, 2);

            if (juryAspect.IsPeeked)
            {
                str += "?";
            }
        }
        else
        {
            str += "?";
        }

        text.text = str;
    }
}
