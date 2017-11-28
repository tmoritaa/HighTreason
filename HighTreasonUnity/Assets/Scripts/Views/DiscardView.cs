using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using HighTreasonGame;

public class DiscardView : CardHolderView 
{
    protected override CardHolder RetrieveCardHolder()
    {
        return GameManager.Instance.Game.Discards;
    }
}
