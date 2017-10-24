using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using HighTreasonGame;

public class MulliganUsageTrigger : UsageTrigger
{
    private Image image;

    protected override void Awake()
    {
        base.Awake();

        usageType = Player.PlayerActionParams.UsageType.Mulligan;

        image = GetComponent<Image>();
    }

    void Update()
    {
        if (ChoiceHandlerDelegator.Instance.CurChoiceType != UnityChoiceHandler.ChoiceType.CardAndUsage
            || GameManager.Instance.Game.CurState.StateType != GameState.GameStateType.TrialInChief
            || GameManager.Instance.Game.CurPlayer.PerformedMulligan)
        {
            image.color = Color.grey;
        }
        else
        {
            image.color = Color.white;
        }
    }

    protected override void onClick()
    {
        if (ChoiceHandlerDelegator.Instance.CurChoiceType == UnityChoiceHandler.ChoiceType.CardAndUsage
            && GameManager.Instance.Game.CurState.StateType == GameState.GameStateType.TrialInChief)
        {
            Debug.Log("Mulligan Choice complete");
            ChoiceHandlerDelegator.Instance.ChoiceMade(usageType);
        }
    }
}
