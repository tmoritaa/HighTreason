using System;
using System.Collections.Generic;
using System.Linq;

using HighTreasonGame;

using UnityEngine;

public abstract class HTGOUIElement : MonoBehaviour
{
    protected List<Property> properties = new List<Property>();

    protected virtual void Awake()
    {
        HTGameObject htgoElement = GameManager.Instance.Game.GetHTGOFromCondition(
            (HTGameObject htgo) => {
                bool retVal = true;
                properties.ForEach(p => retVal &= htgo.Properties.Contains(p));
                return retVal;
            })[0];

        setHTGOElement(htgoElement);

        GameManager.Instance.notifyInitGame += initUIElement;
    }

    protected abstract void setHTGOElement(HTGameObject htgo);

    protected abstract void initUIElement();
}
