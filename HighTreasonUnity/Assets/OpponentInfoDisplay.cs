using System;
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
        Player p = GameManager.Instance.Game.GetOtherPlayer();

        background.color = (p.Side == Player.PlayerSide.Prosecution) ? new Color(1, 0, 0) : new Color(0, 0, 1);

        roleText.text = p.Side.ToString();
        handText.text = "Hand Size=" + p.Hand.Cards.Count.ToString();
        summationDeckText.text = "Summation Size=" + p.SummationDeck.Cards.Count.ToString();
    }
}
