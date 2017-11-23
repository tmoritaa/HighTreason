using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using HighTreasonGame;

public class MulliganUsageTrigger : MonoBehaviour
{
    private Image image;

    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(onClick);
        image = GetComponent<Image>();
    }

    void Update()
    {
        if (canMulligan())
        {
            image.color = Color.white;
        }
        else
        {
            image.color = Color.grey;
        }
    }

    protected void onClick()
    {
        if (canMulligan())
        {
            Debug.Log("Mulligan Choice complete");
            ChoiceHandlerDelegator.Instance.ChoiceMade(Player.PlayerActionParams.UsageType.Mulligan);
        }
    }

    private bool canMulligan()
    {
        return ChoiceHandlerDelegator.Instance.CurChoiceType == UnityChoiceHandler.ChoiceType.CardAndUsage
            && GameManager.Instance.Game.CurState.StateType == GameState.GameStateType.TrialInChief
            && !GameManager.Instance.Game.CurPlayer.PerformedMulligan;
    }
}
