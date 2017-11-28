using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using HighTreasonGame;

using UnityEngine;

public class DetailedCardView : MonoBehaviour 
{
    [SerializeField]
    DetailedCardElement cardElement;

    public void SetCardForDisplay(Card card)
    {
        cardElement.SetCardTemplate(card);
    }
}
