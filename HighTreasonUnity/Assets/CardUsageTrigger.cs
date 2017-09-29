using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using HighTreasonGame;

using UnityEngine;
using UnityEngine.UI;

public abstract class CardUsageTrigger : MonoBehaviour 
{
    protected Player.CardUsageParams.UsageType usageType;

    protected virtual void Awake()
    {
        GetComponent<Button>().onClick.AddListener(onClick);
    }

    protected abstract void onClick();
}
