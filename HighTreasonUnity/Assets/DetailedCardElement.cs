using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using HighTreasonGame;

using UnityEngine;
using UnityEngine.UI;

public class DetailedCardElement : MonoBehaviour 
{
    [SerializeField]
    private Text typing;

    [SerializeField]
    private Text cardName;

    [SerializeField]
    private Text actionPoints;

    [SerializeField]
    private GameObject jurySelectionParent;

    [SerializeField]
    private GameObject trialInChiefParent;

    [SerializeField]
    private GameObject summationParent;

    [SerializeField]
    private EventCardUsageTrigger eventCardUsageTriggerPrefab;

    private CardTemplate displayedCard;

    public void SetCardTemplate(CardTemplate card)
    {
        displayedCard = card;

        updateDisplay();
    }

    private void updateDisplay()
    {
        resetDisplay();

        var cardInfo = CardInfoManager.Instance.GetCardInfo(displayedCard.Name);

        typing.text = cardInfo.typing;
        cardName.text = cardInfo.name;
        actionPoints.text = displayedCard.ActionPts.ToString();

        GameState.GameStateType[] stateTypes = new GameState.GameStateType[] { GameState.GameStateType.JurySelection, GameState.GameStateType.TrialInChief, GameState.GameStateType.Summation };
        List<string>[] cardTexts = new List<string>[] { cardInfo.jurySelectionTexts, cardInfo.trialInChiefTexts, cardInfo.summationTexts };
        GameObject[] parentGOs = new GameObject[] { jurySelectionParent, trialInChiefParent, summationParent };

        for (int j = 0; j < cardTexts.Length; ++j)
        {
            List<string> textList = cardTexts[j];
            GameObject parentGO = parentGOs[j];

            int size = textList.Count;
            for (int i = 0; i < textList.Count; ++i)
            {
                EventCardUsageTrigger eventObj = GameObject.Instantiate(eventCardUsageTriggerPrefab);
                eventObj.gameObject.SetActive(true);

                eventObj.Init(stateTypes[j], i);

                Text text = eventObj.GetComponent<Text>();
                text.text = textList[i];

                text.resizeTextMinSize = 30;
                text.resizeTextMaxSize = 50;
                text.resizeTextForBestFit = true;

                text.color = Color.black;

                RectTransform rect = eventObj.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0, 1.0f - (float)(i + 1) / size);
                rect.anchorMax = new Vector2(1, 1.0f - (float)i / size);

                eventObj.transform.SetParent(parentGO.transform, false);
            }
        }
    }

    private void resetDisplay()
    {
        GameObject[] parentGOs = new GameObject[] { jurySelectionParent, trialInChiefParent, summationParent };

        foreach (GameObject go in parentGOs)
        {
            foreach (Transform child in go.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
