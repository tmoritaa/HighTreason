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

    private UnityChoiceHandler curChoiceHandler;

    private bool checkIfShouldSkip = false;

    void Awake()
    {
        ChoiceHandlerDelegator.instance = this;
        CurChoiceType = UnityChoiceHandler.ChoiceType.NoChoice;
    }

    void Update()
    {
        if (checkIfShouldSkip)
        {
            object[] validOutput;
            if (inputHandler.SkipChoiceIfNoValid(out validOutput))
            {
                Debug.Log("Skipping choice since no valid choices");
                cleanupAfterChoice();
                curChoiceHandler.ChoiceInputMade(validOutput);
            }

            checkIfShouldSkip = false;
        }

        if (inputHandler != null)
        {
            inputHandler.OnUpdate();
        }
    }

    public void TriggerChoice(UnityChoiceHandler choiceHandler, UnityChoiceHandler.ChoiceType choiceType, params object[] additionalParams)
    {
        CurChoiceType = choiceType;
        curChoiceHandler = choiceHandler;

        switch (CurChoiceType)
        {
            case UnityChoiceHandler.ChoiceType.CardAndUsage:
                Debug.Log("CardAndUsage choice triggered");
                inputHandler = new CardAndUsageInputHandler(additionalParams);
                break;
            case UnityChoiceHandler.ChoiceType.PickBoardObject:
                Debug.Log("PickBoardObject choice triggered");
                inputHandler = new PickBoardObjectInputHandler(additionalParams);
                break;
        }

        checkIfShouldSkip = true;
    }

    public void ChoiceMade(params object[] input)
    {
        if (inputHandler == null)
        {
            return;
        }

        object[] validOutput;
        bool complete = inputHandler.VerifyInput(out validOutput, input);
        if (complete)
        {
            Debug.Log("ChoiceMade complete");
            cleanupAfterChoice();
            curChoiceHandler.ChoiceInputMade(validOutput);
        }
    }

    private void cleanupAfterChoice()
    {
        CurChoiceType = UnityChoiceHandler.ChoiceType.NoChoice;
        resetChoiceUI();
        inputHandler = null;
    }

    private void resetChoiceUI()
    {
        ViewManager.Instance.HideAllFullscreenViews();
        BoardObjectElementManager.Instance.MarkAllAsUnselectable();
    }
}
