using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using HighTreasonGame;

public class CardElement : MonoBehaviour 
{
    public CardTemplate CardTemplate
    {
        get; private set;
    }

    public void SetCardTemplate(CardTemplate _cardTemplate)
    {
        CardTemplate = _cardTemplate;

        updateDisplay();
    }

    private void updateDisplay()
    {
        // TODO: temp.
        GetComponentInChildren<Text>().text = CardTemplate.Name;
    }
}
