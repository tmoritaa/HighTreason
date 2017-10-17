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

    private UnityChoiceHandler curChoiceHandler;
    public UnityChoiceHandler.ChoiceType CurChoiceType {
        get; private set;
    }

    List<BoardObject> curChoices;
    bool highlightChoices = false;

    void Awake()
    {
        ChoiceHandlerDelegator.instance = this;
        CurChoiceType = UnityChoiceHandler.ChoiceType.NoChoice;
    }

    // TODO: think of a better way to do this.
    void Update()
    {
        if (highlightChoices)
        {
            ViewManager.Instance.HighlightChoices(curChoices);
            highlightChoices = false;
        }
    }

    public void TriggerChoice(UnityChoiceHandler choiceHandler, UnityChoiceHandler.ChoiceType choiceType, params object[] additionalParams)
    {
        curChoiceHandler = choiceHandler;
        CurChoiceType = choiceType;

        if (CurChoiceType == UnityChoiceHandler.ChoiceType.PickBoardObject)
        {
            curChoices = (List<BoardObject>)additionalParams[0];
            highlightChoices = true;
        }
    }

    public void ChoiceComplete(params object[] input)
    {
        if (curChoiceHandler != null)
        {
            resetViews();

            curChoiceHandler.ChoiceInputMade(input);
            curChoiceHandler = null;
            CurChoiceType = UnityChoiceHandler.ChoiceType.NoChoice;
        }
    }

    private void resetViews()
    {
        ViewManager.Instance.HideDetailedCardView();
    }
}
