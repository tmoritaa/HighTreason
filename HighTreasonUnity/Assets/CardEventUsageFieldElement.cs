﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using HighTreasonGame;

using UnityEngine;
using UnityEngine.UI;

public class CardEventUsageFieldElement : CardUsageFieldElement 
{
    private GameState.GameStateType usableState;

    private int eventIdx;

    private Card card;

    public void Init(Card _card, GameState.GameStateType _usableState, int _eventIdx, CardInfo.EffectPair ep)
    {
        card = _card;
        usableState = _usableState;
        eventIdx = _eventIdx;

        this.GetComponentInChildren<Text>().text = ep.Text;

        Color color = Color.black;
        switch (ep.Type)
        {
            case CardInfo.EffectPair.EffectType.Prosecution:
                color = ViewManager.Instance.ProsecutionColor;
                break;
            case CardInfo.EffectPair.EffectType.Defense:
                color = ViewManager.Instance.DefenseColor;
                break;
            case CardInfo.EffectPair.EffectType.Neutral:
                color = ViewManager.Instance.NeutralColor;
                break;
            case CardInfo.EffectPair.EffectType.JurySelect:
                color = ViewManager.Instance.JurySelectColor;
                break;
        }
        this.GetComponent<Image>().color = color;
    }

    protected override void onValidClick()
    {
        Debug.Log("Event Choice complete");
        ChoiceHandlerDelegator.Instance.ChoiceMade(Player.PlayerActionParams.UsageType.Event, card, eventIdx);
    }

    protected override bool canUse()
    {
        return
            ChoiceHandlerDelegator.Instance.CurChoiceType == UnityChoiceHandler.ChoiceType.CardAndUsage
            && GameManager.Instance.Game.CurState.StateType == usableState
            && card.CanBePlayed;
    }
}
