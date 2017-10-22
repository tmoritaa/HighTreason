using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using HighTreasonGame;

public class HandHandler : MonoBehaviour 
{
    [SerializeField]
    private MiniCardElement cardElementPrefab;

    private Player displayingPlayer;

    private List<MiniCardElement> cardElements = new List<MiniCardElement>();

    private Image background; 

	void Awake()
	{
        EventDelegator.Instance.NotifyStateStart += updateDisplayingPlayer;
        EventDelegator.Instance.NotifyStartOfTurn += updateDisplayingPlayer;
        EventDelegator.Instance.NotifyPlayedCard += cardPlayed;
	}

    void Start()
    {
        background = GetComponent<Image>();
    }

    private void cardPlayed(Player.CardUsageParams usageParams)
    {
        MiniCardElement playedCard = cardElements.Find(ce => ce.cardObj == usageParams.card);

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

            background.color = (curPlayer.Side == Player.PlayerSide.Prosecution) ? new Color(1, 0, 0) : new Color(0, 0, 1);

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

        foreach (Card card in displayingPlayer.Hand.Cards)
        {
            MiniCardElement element = GameObject.Instantiate<MiniCardElement>(cardElementPrefab);
            element.transform.SetParent(this.transform, false);
            cardElements.Add(element);

            element.SetCard(card);
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
