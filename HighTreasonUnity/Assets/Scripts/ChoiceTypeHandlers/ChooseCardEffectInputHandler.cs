using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using HighTreasonGame;

public class ChooseCardEffectInputHandler : ChoiceTypeInputHandler
{
    private Card card;
    private bool showView = true;

    public ChooseCardEffectInputHandler(Card _card, string desc)
        : base(UnityChoiceHandler.ChoiceType.ChooseCardEffect, desc, false)
    {
        card = _card;
    }

    public override bool VerifyInput(out object[] validOutput, params object[] input)
    {
        validOutput = input;
        return true;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (showView)
        {
            ViewManager.Instance.DisplayView(ViewManager.PopupType.DetailedCard, false, card);
            showView = false;
        }
    }
}
