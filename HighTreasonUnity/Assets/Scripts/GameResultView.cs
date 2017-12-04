using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using HighTreasonGame;

public class GameResultView : MonoBehaviour 
{
    [SerializeField]
    Text text;

    public void InitInfo(Player.PlayerSide winningPlayer, bool notEnoughGuiltVictory, int finalScore)
    {
        if (notEnoughGuiltVictory)
        {
            text.text = "Defense player has won by not enough guilt";
        }
        else
        {
            text.text = winningPlayer + " has won with " + finalScore + " points";
        }

    }
}
