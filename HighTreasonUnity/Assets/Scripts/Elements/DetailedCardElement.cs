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
    private Text notes;

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

    public void SetCardTemplate(Card card)
    {
        updateDisplay(card);
    }

    private void updateDisplay(Card card)
    {
        resetDisplay();

        var cardInfo = card.Template.CardInfo;

        typing.text = cardInfo.Typing;
        cardName.text = cardInfo.Name;
        notes.text = cardInfo.Notes;
        actionPoints.Init(card);

        GameState.GameStateType[] stateTypes = new GameState.GameStateType[] { GameState.GameStateType.JurySelection, GameState.GameStateType.TrialInChief, GameState.GameStateType.Summation };
        List<CardInfo.EffectInfo>[] cardEffectInfos = new List<CardInfo.EffectInfo>[] { cardInfo.JurySelectionInfos, cardInfo.TrialInChiefInfos, cardInfo.SummationInfos };
        List<CardTemplate.CardEffectPair>[] cardEffectPairs = new List<CardTemplate.CardEffectPair>[] { card.Template.SelectionEvents, card.Template.TrialEvents, card.Template.SummationEvents };
        GameObject[] parentGOs = new GameObject[] { jurySelectionParent, trialInChiefParent, summationParent };

        for (int j = 0; j < stateTypes.Length; ++j)
        {
            List<CardInfo.EffectInfo> infoList = cardEffectInfos[j];
            List<CardTemplate.CardEffectPair> effectPairs = cardEffectPairs[j];
            GameObject parentGO = parentGOs[j];
            GameState.GameStateType stateType = stateTypes[j];

            int size = infoList.Count;
            for (int i = 0; i < size; ++i)
            {
                CardEventUsageFieldElement eventObj = GameObject.Instantiate(cardEventFieldElementPrefab);
                eventObj.gameObject.SetActive(true);

                eventObj.Init(card, stateType, i, infoList[i], effectPairs[i].Selectable);

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
