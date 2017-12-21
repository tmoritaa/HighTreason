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
        public int eventIdx = -1;

        public PlayerActionParams()
        {}

        // Copy constructor
        public PlayerActionParams(PlayerActionParams actionParams, Game game)
        {
            usage = actionParams.usage;
            card = game.FindCard(actionParams.card);
            eventIdx = actionParams.eventIdx;
        }

        public bool CheckCloneEquality(PlayerActionParams actionParams)
        {
            bool equal = true;

            equal &= !object.ReferenceEquals(this, actionParams);
            if (!equal)
            {
                Console.WriteLine("PlayerActionParams references test failed");
                return equal;
            }

            equal &= usage == actionParams.usage;
            if (!equal)
            {
                Console.WriteLine("PlayerActionParams usage was not equal");
                return equal;
            }

            equal &= card.CheckCloneEquality(actionParams.card);
            if (!equal)
            {
                Console.WriteLine("PlayerActionParams card was not equal");
                return equal;
            }

            equal &= eventIdx == actionParams.eventIdx;
            if (!equal)
            {
                Console.WriteLine("PlayerActionParams eventIdx was not equal");
                return equal;
            }

            return equal;
        }

        public string ToString(Player choosingPlayer, GameState.GameStateType curStateType)
        {
            string str = "";

            switch (usage)
            {
                case UsageType.Event:
                    {
                        str += choosingPlayer.Side + " played " + card.Template.Name + " as event\n";

                        if (curStateType == GameState.GameStateType.JurySelection)
                        {
                            str += card.Template.CardInfo.JurySelectionInfos[eventIdx].Text;
                        }
                        else if (curStateType == GameState.GameStateType.TrialInChief)
                        {
                            str += card.Template.CardInfo.TrialInChiefInfos[eventIdx].Text;
                        }
                        else if (curStateType == GameState.GameStateType.Summation)
                        {
                            str += card.Template.CardInfo.SummationInfos[eventIdx].Text;
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
