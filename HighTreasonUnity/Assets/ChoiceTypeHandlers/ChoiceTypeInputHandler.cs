using System;
using System.Collections.Generic;
using System.Linq;

using HighTreasonGame;

public abstract class ChoiceTypeInputHandler
{
    public UnityChoiceHandler.ChoiceType choiceType
    {
        get; protected set;
    }

    public ChoiceTypeInputHandler(UnityChoiceHandler.ChoiceType _choiceType)
    {
        choiceType = _choiceType;
    }

    public virtual bool SkipChoiceIfNoValid(out object[] emptyOutput)
    {
        emptyOutput = new object[] { };
        return false;
    }

    public abstract bool VerifyInput(out object[] validOutput, params object[] input);

    public abstract void OnUpdate();
}
