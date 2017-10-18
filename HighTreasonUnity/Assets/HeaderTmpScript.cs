using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using HighTreasonGame;

using UnityEngine;
using UnityEngine.UI;

public class HeaderTmpScript : MonoBehaviour 
{
    private Text text;

	void Start()
	{
        text = this.GetComponent<Text>();
	}
	
	void Update()
	{
        text.text = GameManager.Instance.Game.CurPlayer.Side.ToString();
    }
}
