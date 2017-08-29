using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using HighTreasonGame;

public class HandHandler : MonoBehaviour 
{
    [SerializeField]
    private CardElement cardElementPrefab;

    private Player displayingPlayer;

    private List<CardElement> cardElements = new List<CardElement>();

	void Awake()
	{
        GameManager.Instance.Game.NotifyStateStart += updateDisplayingPlayer;
        GameManager.Instance.Game.NotifyStartOfTurn += updateDisplayingPlayer;
        GameManager.Instance.Game.NotifyPlayedCard += cardPlayed;
	}

    private void cardPlayed(Player.CardUsageParams usageParams)
    {
        CardElement playedCard = cardElements.Find(ce => ce.CardTemplate == usageParams.card);
        GameObject.Destroy(playedCard.gameObject);

        cardElements.Remove(playedCard);

        updateCardPositions();
    }

    private void updateDisplayingPlayer()
    {
        Player curPlayer = GameManager.Instance.Game.CurPlayer;

        if (curPlayer.ChoiceType == Player.PlayerType.Human && curPlayer != displayingPlayer)
        {
            displayingPlayer = curPlayer;

            initHandDisplay();
        }
    }

    private void updateCardPositions()
    {
        float cardWidth = cardElementPrefab.GetComponent<RectTransform>().rect.width;
        float margin = 30f;
        float distBtwnCards = cardWidth + margin;

        float startingXPos = -((int)(cardElements.Count / 2) * (distBtwnCards));
        if (cardElements.Count % 2 == 0)
        {
            startingXPos += (cardWidth + margin) / 2.0f;
        }

        for (int i = 0; i < cardElements.Count; ++i)
        {
            Vector2 localPos = new Vector2(startingXPos + i * distBtwnCards, 0);
            cardElements[i].gameObject.transform.localPosition = localPos;
        }
    }

    private void initHandDisplay()
    {
        cleanup();

        foreach (CardTemplate card in displayingPlayer.Hand)
        {
            CardElement element = GameObject.Instantiate<CardElement>(cardElementPrefab);
            element.transform.SetParent(this.transform, false);
            cardElements.Add(element);

            element.SetCardTemplate(card);
        }

        updateCardPositions();
    }

    private void cleanup()
    {
        foreach (CardElement element in cardElements)
        {
            GameObject.Destroy(element.gameObject);
        }

        cardElements.Clear();
    }
}
