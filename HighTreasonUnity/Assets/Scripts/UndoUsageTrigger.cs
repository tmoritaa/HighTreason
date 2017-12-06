using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using HighTreasonGame;

public class UndoUsageTrigger : MonoBehaviour 
{
    private Image image;

    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(onClick);
        image = GetComponent<Image>();
    }

    void Update()
    {
        if (canUndo())
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
        if (canUndo())
        {
            Debug.Log("Undo Choice complete");
            ChoiceHandlerDelegator.Instance.ChoiceMade("cancel");
        }
    }

    private bool canUndo()
    {
        return (ChoiceHandlerDelegator.Instance.CurChoiceType == UnityChoiceHandler.ChoiceType.ChooseBoardObjects && GameManager.Instance.Game.CurState.StateType != GameState.GameStateType.JuryDismissal)
            || ChoiceHandlerDelegator.Instance.CurChoiceType == UnityChoiceHandler.ChoiceType.MomentOfInsight
            || ChoiceHandlerDelegator.Instance.CurChoiceType == UnityChoiceHandler.ChoiceType.ChooseCards;
    }
}
