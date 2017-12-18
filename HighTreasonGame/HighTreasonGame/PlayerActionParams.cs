using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame
{
    public class PlayerActionParams
    {
        public enum UsageType
        {
            Event,
            Action,
            Mulligan,
            Cancelled,
        }

        public Card card;
        public UsageType usage;
        public List<object> misc = new List<object>();

        public string ToString(Player choosingPlayer, GameState.GameStateType curStateType)
        {
            string str = "";

            switch (usage)
            {
                case UsageType.Event:
                    {
                        str += choosingPlayer.Side + " played " + card.Template.Name + " as event\n";

                        int idx = (int)misc[0];
                        if (curStateType == GameState.GameStateType.JurySelection)
                        {
                            str += card.Template.CardInfo.JurySelectionInfos[idx].Text;
                        }
                        else if (curStateType == GameState.GameStateType.TrialInChief)
                        {
                            str += card.Template.CardInfo.TrialInChiefInfos[idx].Text;
                        }
                        else if (curStateType == GameState.GameStateType.Summation)
                        {
                            str += card.Template.CardInfo.SummationInfos[idx].Text;
                        }
                    }
                    break;
                case UsageType.Action:
                    str += choosingPlayer.Side + " played " + card.Template.Name + " as action for " + card.Template.ActionPts + " points";
                    break;
                case UsageType.Mulligan:
                    str += choosingPlayer.Side + " mulliganed";
                    break;
            }

            return str;
        }
    }
}
