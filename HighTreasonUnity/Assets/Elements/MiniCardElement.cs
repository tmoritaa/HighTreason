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

    public Card cardObj
    {
        get; private set;
    }

    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(onClick);
    }

    void onClick()
    {
        ViewManager.Instance.DisplayView(ViewManager.PopupType.DetailedCard, cardObj);
    }

    public void SetCard(Card _cardObj)
    {
        cardObj = _cardObj;

        updateDisplay();
    }

    private void updateDisplay()
    {
        Debug.Log("Setting up card " + cardObj.Template.Name);
        var cardInfo = CardInfoManager.Instance.GetCardInfo(cardObj.Template.Name);

        typing.text = cardInfo.typing;
        cardName.text = cardInfo.name;
    }
}
