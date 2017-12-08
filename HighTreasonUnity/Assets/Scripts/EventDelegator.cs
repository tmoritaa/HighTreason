using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using HighTreasonGame;

public class EventDelegator : MonoBehaviour 
{
    private class GameResultArgs
    {
        public Player.PlayerSide winningPlayer;
        public bool notEnoughGuiltVictory;
        public int finalScore;
    }

    private static EventDelegator instance;

    public static EventDelegator Instance
    {
        get { return instance; }
    }

    public Game.StateStartEvent NotifyStateStart;
    public Game.StartOfTurnEvent NotifyStartOfTurn;
    public Game.PlayedCardEvent NotifyPlayerActionPerformed;
    public Game.GameEndEvent NotifyGameEnd;

    private bool doNotifyStateStart = false;
    private bool doNotifyStartOfTurn = false;
    private bool doNotifyPlayerActionPerformed = false;
    private bool doNotifyGameEnd = false;

    private ChoiceHandler.PlayerActionParams usageParamsArg;
    private GameResultArgs gameResultArg;

    void Awake()
    {
        EventDelegator.instance = this;

        GameManager.Instance.Game.NotifyStateStart += triggerStateStart;
        GameManager.Instance.Game.NotifyStartOfTurn += triggerStartOfTurn;
        GameManager.Instance.Game.NotifyPlayerActionPerformed += triggerPlayerActionPerformed;
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

        if (doNotifyPlayerActionPerformed)
        {
            if (NotifyPlayerActionPerformed != null)
            {
                NotifyPlayerActionPerformed(usageParamsArg);
            }
            doNotifyPlayerActionPerformed = false;
            usageParamsArg = null;
        }

        if (doNotifyGameEnd)
        {
            if (NotifyGameEnd != null)
            {
                NotifyGameEnd(gameResultArg.winningPlayer, gameResultArg.notEnoughGuiltVictory, gameResultArg.finalScore);
            }
            doNotifyGameEnd = false;
            gameResultArg = null;
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

    private void triggerPlayerActionPerformed(ChoiceHandler.PlayerActionParams usageParams)
    {
        doNotifyPlayerActionPerformed = true;
        usageParamsArg = usageParams;
    }

    private void triggerGameEnd(Player.PlayerSide winningPlayerSide, bool winByNotEnoughGuilt, int finalScore)
    {
        doNotifyGameEnd = true;
        gameResultArg = new GameResultArgs();
        gameResultArg.winningPlayer = winningPlayerSide;
        gameResultArg.notEnoughGuiltVictory = winByNotEnoughGuilt;
        gameResultArg.finalScore = finalScore;
    }
}
