using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighTreasonGame
{
    public class Utility
    {
        public static CardTemplate.EffectType ConvertToEffectTypeFromStateType(GameState.StateType stateType)
        {
            CardTemplate.EffectType effectType = CardTemplate.EffectType.Invalid;

            switch (stateType)
            {
                case GameState.StateType.JurySelection:
                    effectType = CardTemplate.EffectType.Selection;
                    break;
                case GameState.StateType.TrialInChief:
                    effectType = CardTemplate.EffectType.Trial;
                    break;
                case GameState.StateType.Summation:
                    effectType = CardTemplate.EffectType.Summation;
                    break;
            }

            System.Diagnostics.Debug.Assert(effectType != CardTemplate.EffectType.Invalid, "Effect Type after conversion from GameState Type should never be invalid.");

            return effectType;
        }

    }
}
