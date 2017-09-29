using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using HighTreasonGame;

public class MiniCardElement : MonoBehaviour 
{
    [SerializeField]
    private Text typing;
    [SerializeField]
    private Text cardName;

    public CardTemplate CardTemplate
    {
        get; private set;
    }

    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(onClick);
    }

    void onClick()
    {
        ViewManager.Instance.DisplayDetailedCardViewWithCard(CardTemplate);
    }

    public void SetCardTemplate(CardTemplate _cardTemplate)
    {
        CardTemplate = _cardTemplate;

        updateDisplay();
    }

    private void updateDisplay()
    {
        Debug.Log("Setting up card " + CardTemplate.Name);
        var cardInfo = CardInfoManager.Instance.GetCardInfo(CardTemplate.Name);

        typing.text = cardInfo.typing;
        cardName.text = cardInfo.name;
    }
}
