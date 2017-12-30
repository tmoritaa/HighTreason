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

        public UsageType usage;
        public Card card;
        public int eventIdx = -1;

        public PlayerActionParams()
        {}

        public PlayerActionParams(UsageType _usage, Card _card = null, int _eventIdx = -1)
        {
            usage = _usage;

            if ((usage == UsageType.Event || usage == UsageType.Action) && _card == null)
            {
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    System.Diagnostics.Debugger.Break();
                }
            }

            card = _card;
            eventIdx = _eventIdx;
        }

        // Copy constructor
        public PlayerActionParams(PlayerActionParams actionParams, Game game)
        {
            usage = actionParams.usage;
            if (actionParams.card != null)
            {
                card = game.FindCard(actionParams.card);
                eventIdx = actionParams.eventIdx;
            }
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
