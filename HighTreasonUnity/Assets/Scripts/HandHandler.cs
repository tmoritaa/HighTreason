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
        EventDelegator.Instance.NotifyPlayerActionPerformed += playerActionPerformed;
	}

    void Start()
    {
        background = GetComponent<Image>();
    }

    void Update()
    {
        if (ChoiceHandlerDelegator.Instance.CurChoosingPlayer != null && displayingPlayer != ChoiceHandlerDelegator.Instance.CurChoosingPlayer)
        {
            updateDisplayingPlayer(ChoiceHandlerDelegator.Instance.CurChoosingPlayer);
        }
    }

    private void playerActionPerformed(ChoiceHandler.PlayerActionParams usageParams)
    {
        initHandDisplay();
    }

    private void updateDisplayingPlayer(Player choosingPlayer)
    {
        Player curPlayer = choosingPlayer;

        if (curPlayer.ChoiceType == Player.PlayerType.Human && curPlayer != displayingPlayer)
        {
            displayingPlayer = curPlayer;

            background.color = (curPlayer.Side == Player.PlayerSide.Prosecution) ? ViewManager.Instance.ProsecutionColor : ViewManager.Instance.DefenseColor;

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
