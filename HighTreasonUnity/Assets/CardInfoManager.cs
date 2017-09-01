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
        public string name;
        public string typing;
        public List<string> jurySelectionTexts = new List<string>();
        public List<string> trialInChiefTexts = new List<string>();
        public List<string> summationTexts = new List<string>();

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
            foreach (string txt in trialInChiefTexts)
            {
                outStr += txt + "\n";
            }
            outStr += "Summation:\n";
            foreach (string txt in summationTexts)
            {
                outStr += txt + "\n";
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

            foreach (var txt in kv.Value["trial_in_chief"])
            {
                cardInfo.trialInChiefTexts.Add((string)txt);
            }

            foreach (var txt in kv.Value["summation"])
            {
                cardInfo.summationTexts.Add((string)txt);
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
