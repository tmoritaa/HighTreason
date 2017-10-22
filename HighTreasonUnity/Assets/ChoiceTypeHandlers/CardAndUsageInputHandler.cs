﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class CardAndUsageInputHandler : ChoiceTypeInputHandler
{
    public CardAndUsageInputHandler(object[] _additionalParams) : base(_additionalParams)
    {}

    public override bool VerifyInput(out object[] validOutput, params object[] input)
    {
        validOutput = input;
        return true;
    }

    public override void OnUpdate()
    {
        // Do Nothing.
    }
}
