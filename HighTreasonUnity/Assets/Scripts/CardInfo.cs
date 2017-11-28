using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class CardInfo
{
    public class EffectPair
    {
        public enum EffectType
        {
            Prosecution,
            Defense,
            Neutral,
            JurySelect,
        };

        public EffectType Type { get; private set; }
        public string Text { get; private set; }

        public EffectPair(string typeStr, string text)
        {
            switch (typeStr)
            {
                case "prosecution":
                    Type = EffectType.Prosecution;
                    break;
                case "defense":
                    Type = EffectType.Defense;
                    break;
                case "neutral":
                    Type = EffectType.Neutral;
                    break;
                default:
                    Debug.Assert(false, "EffectPair instantiation received incorrect typeStr. Should never happen.");
                    Type = EffectType.JurySelect;
                    break;

            }

            Text = text;
        }

        public EffectPair(EffectType type, string text)
        {
            Type = type;
            Text = text;
        }
    }

    public string name;
    public string typing;
    public List<EffectPair> jurySelectionPairs = new List<EffectPair>();
    public List<EffectPair> trialInChiefPairs = new List<EffectPair>();
    public List<EffectPair> summationPairs = new List<EffectPair>();

    public override string ToString()
    {
        string outStr = "";
        outStr += name + "\n";
        outStr += typing + "\n";
        outStr += "JurySelection:\n";
        foreach (EffectPair ep in jurySelectionPairs)
        {
            outStr += ep.Text + "\n";
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
