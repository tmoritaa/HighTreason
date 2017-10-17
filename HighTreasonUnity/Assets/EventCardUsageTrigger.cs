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

    public void Init(CardTemplate _card, GameState.GameStateType _usableState, int _eventIdx)
    {
        card = _card;
        usableState = _usableState;
        eventIdx = _eventIdx;
    }

    protected override void onClick()
    {
        if (ChoiceHandlerDelegator.Instance.CurChoiceType == UnityChoiceHandler.ChoiceType.CardAndUsage 
            && GameManager.Instance.Game.CurState.StateType == usableState)
        {
            Debug.Log("Event Choicecomplete");
            ChoiceHandlerDelegator.Instance.ChoiceComplete(card, usageType, eventIdx);
        }
    }

}
