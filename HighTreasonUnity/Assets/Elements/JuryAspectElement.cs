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

    public void InitJuryAspect(int id)
    {
        List<Property> uniqueProps = new List<Property>() { Property.Jury, Property.Aspect, aspectProp };

        juryAspect = (Jury.JuryAspect)GameManager.Instance.Game.FindBO(
            (BoardObject htgo) => {
                bool retVal = true;
                uniqueProps.ForEach(p => retVal &= htgo.Properties.Contains(p));
                if (retVal)
                {
                    retVal &= ((Jury.JuryAspect)htgo).Owner.Id == id;
                }
                return retVal;
            })[0];

        setupBOAndGO(juryAspect);
        updateUI();
    }

    protected override void init()
    {
        // Do nothing here. Wait until JuryElement calls other initialize function.
    }

    protected override void updateUI()
    {
        string str = string.Empty;
        if (juryAspect.IsVisibleToPlayer(GameManager.Instance.Game.CurPlayer.Side))
        {
            str = juryAspect.Aspect.ToString()[0].ToString();

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
