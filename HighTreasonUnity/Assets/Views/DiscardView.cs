﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using HighTreasonGame;

public class DiscardView : MonoBehaviour 
{
    [SerializeField]
    private int cardsPerCol = 6;

    [SerializeField]
    private float xAnchorStep = 0.2f;

    [SerializeField]
    private float yAnchorStep = 0.1f;

    [SerializeField]
    private MiniCardElement cardPrefab;

    [SerializeField]
    private GameObject cardStartPoint;

    [SerializeField]
    private GameObject cardRoot;

    private List<MiniCardElement> cardElements = new List<MiniCardElement>();

    void OnEnable()
    {
        List<CardTemplate> cards = GameManager.Instance.Game.Discards;

        int numCardsGend = 0;

        foreach (CardTemplate card in cards)
        {
            MiniCardElement cardElement = Instantiate(cardPrefab);
            cardElement.SetCardTemplate(card);

            Vector2 anchor = cardStartPoint.GetComponent<RectTransform>().anchorMin + new Vector2((numCardsGend / cardsPerCol) * xAnchorStep, -(numCardsGend % cardsPerCol) * yAnchorStep);

            RectTransform rect = cardElement.GetComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;

            cardElement.transform.SetParent(cardRoot.transform, false);

            cardElements.Add(cardElement);

            numCardsGend += 1;
        }
    }

    void OnDisable()
    {
        cardElements.ForEach(e => Destroy(e.gameObject));
    }
}