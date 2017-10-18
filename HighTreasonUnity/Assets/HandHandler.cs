using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using HighTreasonGame;

public class HandHandler : MonoBehaviour 
{
    [SerializeField]
    private MiniCardElement cardElementPrefab;

    private Player displayingPlayer;

    private List<MiniCardElement> cardElements = new List<MiniCardElement>();

	void Awake()
	{
        EventDelegator.Instance.NotifyStateStart += updateDisplayingPlayer;
        EventDelegator.Instance.NotifyStartOfTurn += updateDisplayingPlayer;
        EventDelegator.Instance.NotifyPlayedCard += cardPlayed;
	}

    private void cardPlayed(Player.CardUsageParams usageParams)
    {
        MiniCardElement playedCard = cardElements.Find(ce => ce.CardTemplate == usageParams.card);

        if (playedCard != null)
        {
            GameObject.Destroy(playedCard.gameObject);

            cardElements.Remove(playedCard);

            updateCardPositions();
        }
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
            MiniCardElement element = GameObject.Instantiate<MiniCardElement>(cardElementPrefab);
            element.transform.SetParent(this.transform, false);
            cardElements.Add(element);

            element.SetCardTemplate(card);
        }

        updateCardPositions();
    }

    private void cleanup()
    {
        foreach (MiniCardElement element in cardElements)
        {
            GameObject.Destroy(element.gameObject);
        }

        cardElements.Clear();
    }
}
