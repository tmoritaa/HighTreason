using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using HighTreasonGame;

public class MiniCardElement : SelectableElement
{
    [SerializeField]
    private Text typing;
    [SerializeField]
    private Text cardName;

    public Card CardObj
    {
        get
        {
            return (Card)SelectKey;
        }
    }

    public void SetCard(Card _cardObj)
    {
        SelectKey = _cardObj;

        Debug.Log("Setting up card " + CardObj.Template.Name);
        updateUI();
    }

    protected override bool shouldHighlight()
    {
        return ChoiceHandlerDelegator.Instance.CurChoiceType == UnityChoiceHandler.ChoiceType.MomentOfInsight 
            && SelectableElementManager.Instance.KeyIsSelectable(SelectKey);
    }

    protected override bool isSelectable()
    {
        if (ChoiceHandlerDelegator.Instance.CurChoiceType == UnityChoiceHandler.ChoiceType.MomentOfInsight)
        {
            return SelectableElementManager.Instance.KeyIsSelectable(SelectKey);
        }

        return true;
    }

    protected override void onClick()
    {
        if (ChoiceHandlerDelegator.Instance.CurChoiceType == UnityChoiceHandler.ChoiceType.CardAndUsage && cardCanBeDisplayed())
        {
            ViewManager.Instance.DisplayView(ViewManager.PopupType.DetailedCard, CardObj);
        }
        else if (ChoiceHandlerDelegator.Instance.CurChoiceType == UnityChoiceHandler.ChoiceType.MomentOfInsight)
        {
            ChoiceHandlerDelegator.Instance.ChoiceMade(BoardChoices.MomentOfInsightInfo.MomentOfInsightUse.Swap, CardObj);
        }
    }

    protected override void init()
    {
        // Do nothing.
    }

    protected override void updateUI()
    {
        if (CardObj != null)
        {   
            if (cardCanBeDisplayed())
            {
                var cardInfo = CardInfoManager.Instance.GetCardInfo(CardObj.Template.Name);

                typing.text = cardInfo.typing;
                cardName.text = cardInfo.name;
            }
        }
    }

    private bool cardCanBeDisplayed()
    {
        Player curPlayer = GameManager.Instance.Game.CurPlayer;
        return CardObj.Revealed || CardObj.CardHolder.Id == CardHolder.HolderId.Discard || curPlayer.Hand == CardObj.CardHolder || curPlayer.SummationDeck == CardObj.CardHolder;
    }
}
