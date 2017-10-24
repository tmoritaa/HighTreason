using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using HighTreasonGame;

using UnityEngine;

public class EventCardUsageTrigger : UsageTrigger 
{
    private GameState.GameStateType usableState;

    private int eventIdx;

    private Card card;

    protected override void Awake()
    {
        base.Awake();

        usageType = Player.PlayerActionParams.UsageType.Event;
    }

    public void Init(Card _card, GameState.GameStateType _usableState, int _eventIdx)
    {
        card = _card;
        usableState = _usableState;
        eventIdx = _eventIdx;
    }

    protected override void onClick()
    {
        if (ChoiceHandlerDelegator.Instance.CurChoiceType == UnityChoiceHandler.ChoiceType.CardAndUsage 
            && GameManager.Instance.Game.CurState.StateType == usableState
            && card.CanBePlayed)
        {
            Debug.Log("Event Choice complete");
            ChoiceHandlerDelegator.Instance.ChoiceMade(usageType, card, eventIdx);
        }
    }
}
