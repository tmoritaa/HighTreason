using System;
using System.Collections.Generic;
using System.Linq;

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
        public string Description { get; private set; }

        public EffectPair(string typeStr, string text, string desc)
        {
            Text = text;
            Description = desc;

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
                    Type = EffectType.JurySelect;
                    break;

            }
        }

        public EffectPair(EffectType type, string text, string desc)
        {
            Type = type;
            Text = text;
            Description = desc;
        }
    }

    public string Name
    {
        get; private set;
    }

    public string Typing
    {
        get; private set;
    }

    public List<EffectPair> JurySelectionPairs
    {
        get; private set;
    }

    public List<EffectPair> TrialInChiefPairs
    {
        get; private set;
    }

    public List<EffectPair> SummationPairs
    {
        get; private set;
    }

    public CardInfo(string _name, string _typing, List<EffectPair> _jurySelectionPairs, List<EffectPair> _trialInChiefPairs, List<EffectPair> _summationPairs)
    {
        Name = _name;
        Typing = _typing;
        JurySelectionPairs = _jurySelectionPairs;
        TrialInChiefPairs = _trialInChiefPairs;
        SummationPairs = _summationPairs;
    }

    public override string ToString()
    {
        string outStr = "";
        outStr += Name + "\n";
        outStr += Typing + "\n";
        outStr += "JurySelection:\n";
        foreach (EffectPair ep in JurySelectionPairs)
        {
            outStr += ep.Text + "\n";
        }
        outStr += "TrialInChief:\n";
        foreach (EffectPair ep in TrialInChiefPairs)
        {
            outStr += "type:" + ep.Type + " text:" + ep.Text + "\n";
        }
        outStr += "Summation:\n";
        foreach (EffectPair ep in SummationPairs)
        {
            outStr += "type:" + ep.Type + " text:" + ep.Text + "\n";
        }

        return outStr;
    }
}
