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
    private ActionCardUsageTrigger actionPoints;

    [SerializeField]
    private GameObject jurySelectionParent;

    [SerializeField]
    private GameObject trialInChiefParent;

    [SerializeField]
    private GameObject summationParent;

    [SerializeField]
    private EventCardUsageTrigger eventCardUsageTriggerPrefab;

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
        actionPoints.GetComponent<Text>().text = displayedCard.Template.ActionPts.ToString();
        actionPoints.Init(displayedCard);

        GameState.GameStateType[] stateTypes = new GameState.GameStateType[] { GameState.GameStateType.JurySelection, GameState.GameStateType.TrialInChief, GameState.GameStateType.Summation };
        List<object>[] cardObjs = new List<object>[] { cardInfo.jurySelectionTexts.Cast<object>().ToList(), cardInfo.trialInChiefPairs.Cast<object>().ToList(), cardInfo.summationPairs.Cast<object>().ToList() };
        GameObject[] parentGOs = new GameObject[] { jurySelectionParent, trialInChiefParent, summationParent };

        for (int j = 0; j < stateTypes.Length; ++j)
        {
            List<object> objList = cardObjs[j];
            GameObject parentGO = parentGOs[j];
            GameState.GameStateType stateType = stateTypes[j];

            int size = objList.Count;
            for (int i = 0; i < size; ++i)
            {
                EventCardUsageTrigger eventObj = GameObject.Instantiate(eventCardUsageTriggerPrefab);
                eventObj.gameObject.SetActive(true);

                eventObj.Init(displayedCard, stateType, i);

                RectTransform rect = eventObj.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0, 1.0f - (float)(i + 1) / size);
                rect.anchorMax = new Vector2(1, 1.0f - (float)i / size);

                eventObj.transform.SetParent(parentGO.transform, false);

                if (stateType == GameState.GameStateType.JurySelection)
                {
                    eventObj.TextUI.text = (string)objList[i];
                    eventObj.GetComponent<Image>().color = ViewManager.Instance.JurySelectColor;
                }
                else
                {
                    CardInfoManager.CardInfo.EffectPair ep = (CardInfoManager.CardInfo.EffectPair)objList[i];
                    eventObj.TextUI.text = ep.Text;

                    switch (ep.Type)
                    {
                        case CardInfoManager.CardInfo.EffectPair.EffectType.Prosecution:
                            eventObj.GetComponent<Image>().color = ViewManager.Instance.ProsecutionColor;
                            break;
                        case CardInfoManager.CardInfo.EffectPair.EffectType.Defense:
                            eventObj.GetComponent<Image>().color = ViewManager.Instance.DefenseColor;
                            break;
                        case CardInfoManager.CardInfo.EffectPair.EffectType.Neutral:
                            eventObj.GetComponent<Image>().color = ViewManager.Instance.NeutralColor;
                            break;
                    }
                }
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
