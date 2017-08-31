using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class ChoiceHandlerDelegator : MonoBehaviour 
{
    private static ChoiceHandlerDelegator instance;

    public static ChoiceHandlerDelegator Instance
    {
        get { return instance; }
    }

    private UnityChoiceHandler curChoiceHandler;

    void Awake()
    {
        ChoiceHandlerDelegator.instance = this;
    }

    public void TriggerChoice(UnityChoiceHandler choiceHandler)
    {
        curChoiceHandler = choiceHandler;
    }

    public void ChoiceComplete()
    {
        if (curChoiceHandler != null)
        {
            curChoiceHandler.ChoiceInputMade();
            curChoiceHandler = null;
        }
    }
}
