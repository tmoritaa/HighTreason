using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using HighTreasonGame;

public class EventDelegator : MonoBehaviour 
{
    private static EventDelegator instance;

    public static EventDelegator Instance
    {
        get { return instance; }
    }

    public Game.StateStartEvent NotifyStateStart;
    public Game.StartOfTurnEvent NotifyStartOfTurn;
    public Game.PlayedCardEvent NotifyPlayedCard;
    public Game.GameEndEvent NotifyGameEnd;

    private bool doNotifyStateStart = false;
    private bool doNotifyStartOfTurn = false;
    private bool doNotifyPlayedCard = false;
    private bool doNotifyGameEnd = false;

    private Player.CardUsageParams usageParamsArg;
    private Player.PlayerSide winningPlayerArg;

    void Awake()
    {
        EventDelegator.instance = this;

        GameManager.Instance.Game.NotifyStateStart += triggerStateStart;
        GameManager.Instance.Game.NotifyStartOfTurn += triggerStartOfTurn;
        GameManager.Instance.Game.NotifyPlayedCard += triggerPlayedCard;
        GameManager.Instance.Game.NotifyGameEnd += triggerGameEnd;
    }

    void Update()
    {
        if (doNotifyStateStart)
        {
            if (NotifyStateStart != null)
            {
                NotifyStateStart();
            }
            doNotifyStateStart = false;
        }

        if (doNotifyStartOfTurn)
        {
            if (NotifyStartOfTurn != null)
            {
                NotifyStartOfTurn();
            }
            doNotifyStartOfTurn = false;
        }

        if (doNotifyPlayedCard)
        {
            if (NotifyPlayedCard != null)
            {
                NotifyPlayedCard(usageParamsArg);
            }
            doNotifyPlayedCard = false;
            usageParamsArg = null;
        }

        if (doNotifyGameEnd)
        {
            if (NotifyGameEnd != null)
            {
                NotifyGameEnd(winningPlayerArg);
            }
            doNotifyGameEnd = false;
        }
    }

    private void triggerStateStart()
    {
        doNotifyStateStart = true;
    }

    private void triggerStartOfTurn()
    {
        doNotifyStartOfTurn = true;
    }

    private void triggerPlayedCard(Player.CardUsageParams usageParams)
    {
        doNotifyPlayedCard = true;
        usageParamsArg = usageParams;
    }

    private void triggerGameEnd(Player.PlayerSide winningPlayerSide)
    {
        doNotifyGameEnd = true;
        winningPlayerArg = winningPlayerSide;
    }


}
