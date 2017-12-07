using System;
using System.Collections.Generic;
using System.Linq;

using HighTreasonGame;

public abstract class ChoiceTypeInputHandler
{
    public UnityChoiceHandler.ChoiceType ChoiceType
    {
        get; protected set;
    }

    public bool Stoppable
    {
        get; protected set;
    }

    private bool updateUI = false;
    private string description;
    
    public ChoiceTypeInputHandler(UnityChoiceHandler.ChoiceType _choiceType, string desc, bool _stoppable)
    {
        ChoiceType = _choiceType;
        description = desc;
        Stoppable = _stoppable;
        updateUI = true;
    }

    public virtual bool SkipChoiceIfNoValid(out object[] validOutput)
    {
        validOutput = new object[] { };
        return false;
    }

    public virtual void OnUpdate()
    {
        if (updateUI)
        {
            ViewManager.Instance.UpdateActionText(description);
            updateUI = false;
        }
    }

    public abstract bool VerifyInput(out object[] validOutput, params object[] input);
}
