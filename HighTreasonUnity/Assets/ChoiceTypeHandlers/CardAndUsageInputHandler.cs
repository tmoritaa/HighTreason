using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class CardAndUsageInputHandler : ChoiceTypeInputHandler
{
    public CardAndUsageInputHandler(UnityChoiceHandler _curChoiceHandler, object[] _additionalParams) : base(_curChoiceHandler, _additionalParams)
    {}

    public override bool HandleInput(params object[] input)
    {
        curChoiceHandler.ChoiceInputMade(input);
        return true;
    }

    public override void OnUpdate()
    {
        // Do Nothing.
    }
}
