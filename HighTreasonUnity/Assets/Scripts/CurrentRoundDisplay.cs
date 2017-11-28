using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using HighTreasonGame;

public class CurrentRoundDisplay : MonoBehaviour 
{
    [SerializeField]
    Text text;
	
	void Update()
	{
        GameState.GameStateType curState = GameManager.Instance.Game.CurState.StateType;

        string str = string.Empty;
        switch (curState)
        {
            case GameState.GameStateType.JurySelection:
            case GameState.GameStateType.JuryDismissal:
                str = "Jury Selection";
                break;
            case GameState.GameStateType.TrialInChief:
                str = "Trial In Chief";
                break;
            case GameState.GameStateType.Summation:
                str = "Summation";
                break;
            case GameState.GameStateType.Deliberation:
                str = "Deliberation";
                break;

        }
        text.text = str;
	}
}
