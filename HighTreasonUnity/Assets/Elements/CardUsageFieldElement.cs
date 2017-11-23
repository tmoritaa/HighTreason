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

    void Update()
    {
        if (canUse() && !this.highlightGO.activeSelf)
        {
            this.Highlight(true);
        }
        else if (!canUse() && this.highlightGO.activeSelf)
        {
            this.Highlight(false);
        }
    }

    void triggerClick()
    {
        if (canUse())
        {
            onValidClick();
        }
    }

    protected abstract bool canUse();
    protected abstract void onValidClick();
}
