using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using HighTreasonGame;

using UnityEngine;
using UnityEngine.UI;

public abstract class UsageTrigger : MonoBehaviour 
{    
    protected Player.PlayerActionParams.UsageType usageType;

    protected virtual void Awake()
    {
        GetComponent<Button>().onClick.AddListener(onClick);
    }

    protected abstract void onClick();
}
