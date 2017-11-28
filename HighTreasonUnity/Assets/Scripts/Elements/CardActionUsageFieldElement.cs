using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using HighTreasonGame;

using UnityEngine;
using UnityEngine.UI;

public class CardActionUsageFieldElement : CardUsageFieldElement
{
    private Card card;

    public void Init(Card _card)
    {
        card = _card;
        this.GetComponent<Text>().text = card.Template.ActionPts.ToString();
    }

    protected override void onValidClick()
    {
        if (canUse())
        {
            Debug.Log("Action Choice Complete");
            ChoiceHandlerDelegator.Instance.ChoiceMade(Player.PlayerActionParams.UsageType.Action, card);
        }
    }

    protected override bool canUse()
    {
        return 
            (GameManager.Instance.Game.CurState.StateType == GameState.GameStateType.TrialInChief 
            || GameManager.Instance.Game.CurState.StateType == GameState.GameStateType.Summation)
            && ChoiceHandlerDelegator.Instance.CurChoiceType == UnityChoiceHandler.ChoiceType.CardAndUsage
            && card.CanBePlayed;
    }
}
