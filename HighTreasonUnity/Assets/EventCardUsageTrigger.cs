using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using HighTreasonGame;

using UnityEngine;

public class EventCardUsageTrigger : CardUsageTrigger 
{
    private GameState.GameStateType usableState;

    private int eventIdx;

    protected override void Awake()
    {
        base.Awake();

        usageType = Player.CardUsageParams.UsageType.Event;
    }

    public void Init(GameState.GameStateType _usableState, int _eventIdx)
    {
        usableState = _usableState;
        eventIdx = _eventIdx;
    }

    protected override void onClick()
    {
        if (GameManager.Instance.Game.CurState.StateType == usableState)
        {
            ChoiceHandlerDelegator.Instance.ChoiceComplete(usageType, eventIdx);
        }
    }

}
