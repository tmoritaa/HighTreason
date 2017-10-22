using System;
using System.Collections.Generic;
using System.Linq;

using HighTreasonGame;

public abstract class ChoiceTypeInputHandler
{
    protected object[] additionalParams;

    public ChoiceTypeInputHandler(object[] _additionalParams)
    {
        additionalParams = _additionalParams;
    }

    public virtual bool SkipChoiceIfNoValid(out object[] emptyOutput)
    {
        emptyOutput = new object[] { };
        return false;
    }

    public abstract bool VerifyInput(out object[] validOutput, params object[] input);

    public abstract void OnUpdate();
}
