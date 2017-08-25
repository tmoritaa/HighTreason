using System;
using System.Collections.Generic;
using System.Linq;

using HighTreasonGame;

using UnityEngine;

public abstract class GameUIElement : MonoBehaviour
{
    protected bool isInited = false;

    void Update()
    {
        if (isInited)
        {
            updateUIElement();
        }
    }

    protected abstract void updateUIElement();

    public virtual void InitUIElement()
    {
        isInited = true;
    }
    
}
