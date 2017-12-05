using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class CardAndUsageInputHandler : ChoiceTypeInputHandler
{
    public CardAndUsageInputHandler() : base(UnityChoiceHandler.ChoiceType.CardAndUsage, "Select action to perform", false)
    {}

    public override bool VerifyInput(out object[] validOutput, params object[] input)
    {
        validOutput = input;
        return true;
    }
}
