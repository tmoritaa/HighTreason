using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using HighTreasonGame;

using UnityEngine;

public class ActionCardUsageTrigger : UsageTrigger 
{
    private Card card;

    protected override void Awake()
    {
        base.Awake();
            
        usageType = Player.PlayerActionParams.UsageType.Action;
    }

    public void Init(Card _card)
    {
        card = _card;
    }

    protected override void onClick()
    {
        if (ChoiceHandlerDelegator.Instance.CurChoiceType == UnityChoiceHandler.ChoiceType.CardAndUsage 
            && GameManager.Instance.Game.CurState.StateType != GameState.GameStateType.JurySelection
            && card.CanBePlayed)
        {
            Debug.Log("Action Choice Complete");
            ChoiceHandlerDelegator.Instance.ChoiceMade(usageType, card);
        }
    }
}
