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
        get
        {
            if (inputHandler != null)
            {
                return inputHandler.ChoiceType;
            }

            return UnityChoiceHandler.ChoiceType.NoChoice;
        }
    }

    private ChoiceTypeInputHandler inputHandler = null;

    private UnityChoiceHandler curChoiceHandler;

    private bool checkIfShouldSkip = false;

    void Awake()
    {
        ChoiceHandlerDelegator.instance = this;
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

    public void TriggerChoice(UnityChoiceHandler choiceHandler, ChoiceTypeInputHandler _inputHandler)
    {
        inputHandler = _inputHandler;
        curChoiceHandler = choiceHandler;

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
        resetChoiceUI();
        inputHandler = null;
    }

    private void resetChoiceUI()
    {
        ViewManager.Instance.HideAllViews();
        SelectableElementManager.Instance.Reset();
    }
}
