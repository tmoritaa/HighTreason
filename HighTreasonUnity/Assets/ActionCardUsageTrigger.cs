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

    protected override void onClick()
    {
        if (GameManager.Instance.Game.CurState.StateType != GameState.GameStateType.JurySelection)
        ChoiceHandlerDelegator.Instance.ChoiceComplete(usageType);
    }
}
