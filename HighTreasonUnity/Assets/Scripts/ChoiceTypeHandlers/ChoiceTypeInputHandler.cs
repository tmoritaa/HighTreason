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

    public ChoiceTypeInputHandler(UnityChoiceHandler.ChoiceType _choiceType)
    {
        ChoiceType = _choiceType;
    }

    public virtual bool SkipChoiceIfNoValid(out object[] validOutput)
    {
        validOutput = new object[] { };
        return false;
    }

    public abstract bool VerifyInput(out object[] validOutput, params object[] input);

    public abstract void OnUpdate();
}
