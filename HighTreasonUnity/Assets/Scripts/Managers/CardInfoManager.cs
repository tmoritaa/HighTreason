using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEngine;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class CardInfoManager : MonoBehaviour 
{
    private static CardInfoManager instance;
    public static CardInfoManager Instance
    {
        get { return instance; }
    }

    private Dictionary<string, CardInfo> cardInfos = new Dictionary<string, CardInfo>();

    void Awake()
    {
        instance = this;
    }

    void Start()
	{
        TextAsset cardInfoTxt = Resources.Load("HighTreasonCardTexts") as TextAsset;
        JObject root = JObject.Parse(cardInfoTxt.text);

        foreach (var kv in root)
        {
            CardInfo cardInfo = new CardInfo();
            cardInfo.name = kv.Key;
            cardInfo.typing = (string)kv.Value["typing"];
            
            foreach (string text in kv.Value["jury_selection"])
            {
                cardInfo.jurySelectionPairs.Add(new CardInfo.EffectPair(CardInfo.EffectPair.EffectType.JurySelect, text));
            }

            foreach (JObject eo in kv.Value["trial_in_chief"])
            {
                cardInfo.trialInChiefPairs.Add(new CardInfo.EffectPair(eo.Value<string>("type"), eo.Value<string>("text")));
            }

            foreach (JObject eo in kv.Value["summation"])
            {
                cardInfo.summationPairs.Add(new CardInfo.EffectPair(eo.Value<string>("type"), eo.Value<string>("text")));
            }

            cardInfos[cardInfo.name] = cardInfo;
        }
	}

    public CardInfo GetCardInfo(string name)
    {
        CardInfo cardInfo = null;

        if (cardInfos.ContainsKey(name))
        {
            cardInfo = cardInfos[name];
        }
        // TODO: temporary while not all cards implemented. Once enough implemented, can remove.
        else if (cardInfos.ContainsKey(name.Substring(0, name.Length - 2)))
        {
            cardInfo = cardInfos[name.Substring(0, name.Length - 2)];
        }

        return cardInfo;
    }
}
