using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using HighTreasonGame;

using UnityEngine;

public class ChoiceHandlerDelegator : MonoBehaviour 
{
    private static ChoiceHandlerDelegator instance;

    public static ChoiceHandlerDelegator Instance
    {
        get { return instance; }
    }

    public UnityChoiceHandler.ChoiceType CurChoiceType {
        get; private set;
    }

    private ChoiceTypeInputHandler inputHandler;

    void Awake()
    {
        ChoiceHandlerDelegator.instance = this;
        CurChoiceType = UnityChoiceHandler.ChoiceType.NoChoice;
    }

    void Update()
    {
        if (inputHandler != null)
        {
            inputHandler.OnUpdate();
        }
    }

    public void TriggerChoice(UnityChoiceHandler choiceHandler, UnityChoiceHandler.ChoiceType choiceType, params object[] additionalParams)
    {      
        CurChoiceType = choiceType;

        switch (CurChoiceType)
        {
            case UnityChoiceHandler.ChoiceType.CardAndUsage:
                Debug.Log("CardAndUsage choice triggered");
                inputHandler = new CardAndUsageInputHandler(choiceHandler, additionalParams);
                break;
            case UnityChoiceHandler.ChoiceType.PickBoardObject:
                Debug.Log("PickBoardObject choice triggered");
                inputHandler = new PickBoardObjectInputHandler(choiceHandler, additionalParams);
                break;
        }
    }

    public void ChoiceMade(params object[] input)
    {
        if (inputHandler == null)
        {
            return;
        }

        bool complete = inputHandler.HandleInput(input);
        if (complete)
        {
            inputHandler = null;
            CurChoiceType = UnityChoiceHandler.ChoiceType.NoChoice;
            resetViews();
        }
    }

    private void resetViews()
    {
        ViewManager.Instance.HideDetailedCardView();
        ViewManager.Instance.UnhighlightAll();
    }
}
