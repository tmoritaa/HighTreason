using System;
using System.Collections.Generic;

using HighTreasonGame.GameStates;

namespace HighTreasonGame.EventHandlers
{
    public class ConsoleEventHandler : EventHandler
    {
        public override void GotoNextState(Type stateType)
        {
            System.Console.WriteLine("=========================================================================");
            System.Console.WriteLine("Going to state " + stateType);
            System.Console.WriteLine("=========================================================================");
        }

        public override void StartOfNewTurn(Game game, Type stateType)
        {
            string outStr = "\n";

            if (stateType == typeof(JurySelectionState) || stateType == typeof(JuryDismissalState))
            {
                outStr += "Players:\n";
                foreach (Player player in game.GetPlayers())
                {
                    outStr += player;
                }

                outStr += "Discard:\n";
                foreach (CardTemplate card in game.Discards)
                {
                    outStr += card.Name + "\n";
                }

                outStr += "Juries:\n";
                List<Jury> juries = game.Board.Juries;
                foreach(Jury jury in juries)
                {
                    outStr += jury;
                    outStr += "------------------------------------------------\n";
                }
            }
            else
            {
                outStr += game;
            }

            System.Console.WriteLine(outStr);
        }

        public override void PlayedCard(Player player, Player.CardUsageParams cardUsage)
        {
            System.Console.WriteLine("Player " + player.Side + " played " + cardUsage.card.Name + " as event at idx " + (int)cardUsage.misc[0]);
        }
    }
}
