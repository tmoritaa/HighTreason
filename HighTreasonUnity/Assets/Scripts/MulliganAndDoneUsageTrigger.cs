using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using HighTreasonGame;

public class MulliganAndDoneUsageTrigger : MonoBehaviour
{
    private Image image;

    private Text text;

    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(onClick);
        image = GetComponent<Image>();

        text = GetComponentInChildren<Text>();
    }

    void Update()
    {
        if (ChoiceHandlerDelegator.Instance.Stoppable)
        {
            text.text = "Done";
            image.color = Color.white;
        }
        else
        {
            text.text = "Mulligan";

            if (canMulligan())
            {
                image.color = Color.white;
            }
            else
            {
                image.color = Color.grey;
            }
        }
    }

    protected void onClick()
    {
        if (ChoiceHandlerDelegator.Instance.Stoppable)
        {
            Debug.Log("Done Choice complete");
            ChoiceHandlerDelegator.Instance.ChoiceMade("done");
        }
        else if (canMulligan())
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
