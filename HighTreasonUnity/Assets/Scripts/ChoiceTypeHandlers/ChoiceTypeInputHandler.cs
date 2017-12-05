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

    private bool updateDesc = false;
    private string description;
    
    public ChoiceTypeInputHandler(UnityChoiceHandler.ChoiceType _choiceType, string desc, bool _stoppable)
    {
        ChoiceType = _choiceType;
        description = desc;
        Stoppable = _stoppable;
        updateDesc = true;
    }

    public virtual bool SkipChoiceIfNoValid(out object[] validOutput)
    {
        validOutput = new object[] { };
        return false;
    }

    public virtual void OnUpdate()
    {
        if (updateDesc)
        {
            ViewManager.Instance.UpdateActionText(description);
            updateDesc = false;
        }
    }

    public abstract bool VerifyInput(out object[] validOutput, params object[] input);
}
