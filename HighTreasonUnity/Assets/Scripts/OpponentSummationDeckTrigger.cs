using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using HighTreasonGame;

public class OpponentSummationDeckTrigger : HighlightElement 
{
    protected override void Awake()
    {
        base.Awake();

        GetComponent<Button>().onClick.AddListener(onClick);
    }

    protected override bool shouldHighlight()
    {
        return isSelectableForMoI();
    }

    private void onClick()
    {
        if (isSelectableForMoI())
        {
            ChoiceHandlerDelegator.Instance.ChoiceMade(BoardChoices.MomentOfInsightInfo.MomentOfInsightUse.Reveal);
        }
        else
        {
            ViewManager.Instance.DisplayView(ViewManager.PopupType.SummationDeck, true, GameManager.Instance.Game.GetOtherPlayer());
        }
    }

    private bool isSelectableForMoI()
    {
        return ChoiceHandlerDelegator.Instance.CurChoiceType == UnityChoiceHandler.ChoiceType.MomentOfInsight
            && GameManager.Instance.Game.GetOtherPlayer().SummationDeck.Cards.Count > 0;
    }
}
