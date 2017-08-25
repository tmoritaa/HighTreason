using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using HighTreasonGame.GameStates;

public class GameplayBoard : MonoBehaviour 
{
    [SerializeField]
    List<GameUIElement> elements;

    void Awake()
    {
        GameManager.Instance.Game.NotifyStateStart += handleNotifyStateStart;
        this.gameObject.SetActive(false);
    }

    private void handleNotifyStateStart(Type stateType)
    {
        if (stateType == typeof(TrialInChiefState))
        {
            this.gameObject.SetActive(true);
            elements.ForEach(e => e.InitUIElement());
        }
    }
}
