using System;

namespace HighTreasonGame
{
    public abstract class EventHandler
    {
        public abstract void GotoNextState(Type stateType);
        public abstract void StartOfNewTurn(Game game, Type stateType);
        public abstract void PlayedCard(Player player, Player.CardUsageParams usageParams);
    }
}
