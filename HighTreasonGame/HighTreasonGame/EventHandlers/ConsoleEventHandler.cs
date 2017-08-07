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
            // Do nothing.
        }

        public override void PlayedCard(Player player, Player.CardUsageParams cardUsage)
        {
            // TODO: for now. Later once actions are implemented, will have to handle that case.
            System.Console.WriteLine("Player " + player.Side + " played " + cardUsage.card.Name + " as event at idx " + (int)cardUsage.misc[0]);
        }
    }
}
