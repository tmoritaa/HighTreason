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
        EventDelegator.Instance.NotifyStateStart += handleNotifyStateStart;
        this.gameObject.SetActive(false);
    }

    private void handleNotifyStateStart()
    {
        if (GameManager.Instance.Game.CurState.GetType() == typeof(TrialInChiefState))
        {
            this.gameObject.SetActive(true);
            elements.ForEach(e => e.InitUIElement());
        }
    }
}
