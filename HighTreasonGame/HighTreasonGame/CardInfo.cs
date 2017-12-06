using System;
using System.Collections.Generic;
using System.Linq;

public class CardInfo
{
    public class EffectInfo
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
        public string Notes { get; private set; }

        public EffectInfo(string typeStr, string text, string desc)
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

        public EffectInfo(EffectType type, string text, string desc)
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

    public string Notes
    {
        get; private set;
    }

    public List<EffectInfo> JurySelectionInfos
    {
        get; private set;
    }

    public List<EffectInfo> TrialInChiefInfos
    {
        get; private set;
    }

    public List<EffectInfo> SummationInfos
    {
        get; private set;
    }

    public CardInfo(string _name, string _typing, string _notes, List<EffectInfo> _jurySelectionInfo, List<EffectInfo> _trialInChiefInfo, List<EffectInfo> _summationInfo)
    {
        Name = _name;
        Typing = _typing;
        Notes = _notes;
        JurySelectionInfos = _jurySelectionInfo;
        TrialInChiefInfos = _trialInChiefInfo;
        SummationInfos = _summationInfo;
    }

    public override string ToString()
    {
        string outStr = "";
        outStr += Name + "\n";
        outStr += Typing + "\n";
        outStr += "JurySelection:\n";
        foreach (EffectInfo ep in JurySelectionInfos)
        {
            outStr += ep.Text + "\n";
        }
        outStr += "TrialInChief:\n";
        foreach (EffectInfo ep in TrialInChiefInfos)
        {
            outStr += "type:" + ep.Type + " text:" + ep.Text + "\n";
        }
        outStr += "Summation:\n";
        foreach (EffectInfo ep in SummationInfos)
        {
            outStr += "type:" + ep.Type + " text:" + ep.Text + "\n";
        }

        return outStr;
    }
}
