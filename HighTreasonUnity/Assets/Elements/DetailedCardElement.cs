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
    private CardActionUsageFieldElement actionPoints;

    [SerializeField]
    private GameObject jurySelectionParent;

    [SerializeField]
    private GameObject trialInChiefParent;

    [SerializeField]
    private GameObject summationParent;

    [SerializeField]
    private CardEventUsageFieldElement cardEventFieldElementPrefab;

    private Card displayedCard;

    public void SetCardTemplate(Card card)
    {
        displayedCard = card;

        updateDisplay();
    }

    private void updateDisplay()
    {
        resetDisplay();

        var cardInfo = CardInfoManager.Instance.GetCardInfo(displayedCard.Template.Name);

        typing.text = cardInfo.typing;
        cardName.text = cardInfo.name;
        actionPoints.Init(displayedCard);

        GameState.GameStateType[] stateTypes = new GameState.GameStateType[] { GameState.GameStateType.JurySelection, GameState.GameStateType.TrialInChief, GameState.GameStateType.Summation };
        List<CardInfo.EffectPair>[] cardEffectPairs = new List<CardInfo.EffectPair>[] { cardInfo.jurySelectionPairs, cardInfo.trialInChiefPairs, cardInfo.summationPairs };
        GameObject[] parentGOs = new GameObject[] { jurySelectionParent, trialInChiefParent, summationParent };

        for (int j = 0; j < stateTypes.Length; ++j)
        {
            List<CardInfo.EffectPair> pairList = cardEffectPairs[j];
            GameObject parentGO = parentGOs[j];
            GameState.GameStateType stateType = stateTypes[j];

            int size = pairList.Count;
            for (int i = 0; i < size; ++i)
            {
                CardEventUsageFieldElement eventObj = GameObject.Instantiate(cardEventFieldElementPrefab);
                eventObj.gameObject.SetActive(true);

                eventObj.Init(displayedCard, stateType, i, pairList[i]);

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
