using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using HighTreasonGame;

using UnityEngine;
using UnityEngine.UI;

public abstract class CardUsageFieldElement : HighlightElement
{
    protected override void Awake()
    {
        base.Awake();
        GetComponent<Button>().onClick.AddListener(triggerClick);
    }

    void triggerClick()
    {
        if (canUse())
        {
            onValidClick();
        }
    }

    protected override bool shouldHighlight()
    {
        return canUse();
    }

    protected abstract bool canUse();
    protected abstract void onValidClick();
}
