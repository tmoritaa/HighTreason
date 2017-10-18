using System;
using System.Collections.Generic;
using System.Linq;

using HighTreasonGame;

public abstract class ChoiceTypeInputHandler
{
    protected UnityChoiceHandler curChoiceHandler;
    protected object[] additionalParams;

    public ChoiceTypeInputHandler(UnityChoiceHandler _curChoiceHandler, object[] _additionalParams)
    {
        curChoiceHandler = _curChoiceHandler;
        additionalParams = _additionalParams;
    }

    public abstract bool HandleInput(params object[] input);

    public abstract void OnUpdate();
}
