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
        else if (ChoiceHandlerDelegator.Instance.CurChoosingPlayer != null)
        {
            ViewManager.Instance.DisplayView(ViewManager.PopupType.SummationDeck, true, GameManager.Instance.Game.GetOtherPlayer(ChoiceHandlerDelegator.Instance.CurChoosingPlayer));
        }
    }

    private bool isSelectableForMoI()
    {
        return ChoiceHandlerDelegator.Instance.CurChoiceType == UnityChoiceHandler.ChoiceType.MomentOfInsight
            && ChoiceHandlerDelegator.Instance.CurChoosingPlayer != null
            && GameManager.Instance.Game.GetOtherPlayer(ChoiceHandlerDelegator.Instance.CurChoosingPlayer).SummationDeck.Cards.Count > 0;
    }
}
