﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using HighTreasonGame;

public class MiniCardElement : SelectableElement
{
    [SerializeField]
    private Text cardName;

    [SerializeField]
    private Text actionPoints;

    [SerializeField]
    private GameObject eventFieldParent;

    [SerializeField]
    private CardEventUsageFieldElement cardEventFieldElementPrefab;

    public Card DisplayedCard
    {
        get
        {
            return (Card)SelectKey;
        }
    }

    public void SetCard(Card _displayedCard)
    {
        SelectKey = _displayedCard;

        Debug.Log("Setting up card " + _displayedCard.Template.Name);
        initCardDisplay(_displayedCard);
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
        if (ChoiceHandlerDelegator.Instance.CurChoiceType == UnityChoiceHandler.ChoiceType.MomentOfInsight)
        {
            ChoiceHandlerDelegator.Instance.ChoiceMade(BoardChoices.MomentOfInsightInfo.MomentOfInsightUse.Swap, DisplayedCard);
        } else if (cardCanBeDisplayed())
        {
            ViewManager.Instance.DisplayView(ViewManager.PopupType.DetailedCard, DisplayedCard);
        }
    }

    protected override void init()
    {
        // Do nothing.
    }

    protected override void updateUI()
    {
        // Do nothing.
    }

    private void initCardDisplay(Card card)
    {
        resetDisplay();

        if (cardCanBeDisplayed())
        {
            var cardInfo = card.Template.CardInfo;

            cardName.text = cardInfo.Name;
            actionPoints.text = card.Template.ActionPts.ToString();

            bool handled = true;
            GameState.GameStateType stateType = GameManager.Instance.Game.CurState.StateType;
            List<CardInfo.EffectPair> cardEffectPair = null;

            switch (stateType)
            {
                case GameState.GameStateType.JurySelection:
                    cardEffectPair = cardInfo.JurySelectionPairs;
                    break;
                case GameState.GameStateType.TrialInChief:
                    cardEffectPair = cardInfo.TrialInChiefPairs;
                    break;
                case GameState.GameStateType.Summation:
                    cardEffectPair = cardInfo.SummationPairs;
                    break;
                default:
                    handled = false;
                    break;
            }

            if (handled)
            {
                int size = cardEffectPair.Count;
                for (int i = 0; i < size; ++i)
                {
                    CardEventUsageFieldElement eventObj = GameObject.Instantiate(cardEventFieldElementPrefab);
                    eventObj.gameObject.SetActive(true);

                    eventObj.Init(card, stateType, i, cardEffectPair[i], false);

                    RectTransform rect = eventObj.GetComponent<RectTransform>();
                    rect.anchorMin = new Vector2(0, 1.0f - (float)(i + 1) / size);
                    rect.anchorMax = new Vector2(1, 1.0f - (float)i / size);

                    eventObj.transform.SetParent(eventFieldParent.transform, false);
                }
            }
        }
    }

    private void resetDisplay()
    {
        foreach (Transform child in eventFieldParent.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private bool cardCanBeDisplayed()
    {
        Player curPlayer = GameManager.Instance.Game.CurPlayer;
        return DisplayedCard.Revealed || DisplayedCard.CardHolder.Id == CardHolder.HolderId.Discard || curPlayer.Hand == DisplayedCard.CardHolder || curPlayer.SummationDeck == DisplayedCard.CardHolder;
    }
}