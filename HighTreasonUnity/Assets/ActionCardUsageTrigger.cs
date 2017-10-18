using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using HighTreasonGame;

using UnityEngine;

public class ActionCardUsageTrigger : CardUsageTrigger 
{
    protected override void Awake()
    {
        base.Awake();
            
        usageType = Player.CardUsageParams.UsageType.Action;
    }

    public void Init(CardTemplate _card)
    {
        card = _card;
    }

    protected override void onClick()
    {
        if (ChoiceHandlerDelegator.Instance.CurChoiceType == UnityChoiceHandler.ChoiceType.CardAndUsage 
            && GameManager.Instance.Game.CurState.StateType != GameState.GameStateType.JurySelection)
        {
            Debug.Log("Action Choice Complete");
            ChoiceHandlerDelegator.Instance.ChoiceMade(card, usageType);
        }
    }
}
