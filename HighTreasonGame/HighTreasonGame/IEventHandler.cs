using System;

namespace HighTreasonGame
{
    public interface IEventHandler
    {
        void GotoNextState(Type stateType);
        void StartOfNewTurn(Game game, Type stateType);
        void PlayedCard(Player player, Player.CardUsageParams usageParams);
        void GameEnded(Game game, Player.PlayerSide winningPlayerSide);
    }
}
