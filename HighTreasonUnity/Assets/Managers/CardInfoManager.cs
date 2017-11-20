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
    public class CardInfo
    {
        public class EffectPair
        {
            public enum EffectType
            {
                Prosecution,
                Defense,
                Neutral
            };

            public EffectType Type { get; private set; }
            public string Text { get; private set; }

            public EffectPair(string typeStr, string text)
            {
                switch (typeStr)
                {
                    case "prosecution":
                        Type = CardInfo.EffectPair.EffectType.Prosecution;
                        break;
                    case "defense":
                        Type = CardInfo.EffectPair.EffectType.Defense;
                        break;
                    case "neutral":
                        Type = CardInfo.EffectPair.EffectType.Neutral;
                        break;
                }

                Text = text;
            }
        }

        public string name;
        public string typing;
        public List<string> jurySelectionTexts = new List<string>();
        public List<EffectPair> trialInChiefPairs = new List<EffectPair>();
        public List<EffectPair> summationPairs = new List<EffectPair>();

        public override string ToString()
        {
            string outStr = "";
            outStr += name + "\n";
            outStr += typing + "\n";
            outStr += "JurySelection:\n";
            foreach (string txt in jurySelectionTexts)
            {
                outStr += txt + "\n";
            }
            outStr += "TrialInChief:\n";
            foreach (EffectPair ep in trialInChiefPairs)
            {
                outStr += "type:" + ep.Type + " text:" + ep.Text + "\n";
            }
            outStr += "Summation:\n";
            foreach (EffectPair ep in summationPairs)
            {
                outStr += "type:" + ep.Type + " text:" + ep.Text + "\n";
            }

            return outStr;
        }
    }

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
            
            foreach (var txt in kv.Value["jury_selection"])
            {
                cardInfo.jurySelectionTexts.Add((string)txt);
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
