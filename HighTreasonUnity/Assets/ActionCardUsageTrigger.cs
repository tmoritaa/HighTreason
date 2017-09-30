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
        if (GameManager.Instance.Game.CurState.StateType != GameState.GameStateType.JurySelection)
        ChoiceHandlerDelegator.Instance.ChoiceComplete(card, usageType);
    }
}
