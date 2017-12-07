using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using HighTreasonGame;

using UnityEngine;
using UnityEngine.UI;

public class CurrentPlayerTextDisplay : MonoBehaviour 
{
    private Text text;

	void Start()
	{
        text = this.GetComponent<Text>();
	}
	
	void Update()
	{
        if (ChoiceHandlerDelegator.Instance.CurChoosingPlayer != null)
        {
            text.text = ChoiceHandlerDelegator.Instance.CurChoosingPlayer.Side.ToString();
        }
    }
}
