﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using HighTreasonGame;

public class OpponentInfoDisplay : MonoBehaviour 
{
    [SerializeField]
    private Text roleText;

    [SerializeField]
    private Text handText;

    [SerializeField]
    private Text summationDeckText;

    private Image background;

    void Start()
    {
        background = this.GetComponent<Image>();
    }

    void Update()
	{
        if (ChoiceHandlerDelegator.Instance.CurChoosingPlayer != null)
        {
            Player p = GameManager.Instance.Game.GetOtherPlayer(ChoiceHandlerDelegator.Instance.CurChoosingPlayer);

            background.color = (p.Side == Player.PlayerSide.Prosecution) ? ViewManager.Instance.ProsecutionColor : ViewManager.Instance.DefenseColor;

            roleText.text = p.Side.ToString();
            handText.text = "Hand: " + p.Hand.Cards.Count.ToString();
            summationDeckText.text = "Summation: " + p.SummationDeck.Cards.Count.ToString();
        }
    }
}
