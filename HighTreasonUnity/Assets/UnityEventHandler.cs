using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HighTreasonGame;

public class UnityEventHandler : IEventHandler
{
    public void GameEnded(Game game, Player.PlayerSide winningPlayerSide)
    {
        throw new NotImplementedException();
    }

    public void GotoNextState(Type stateType)
    {
        throw new NotImplementedException();
    }

    public void PlayedCard(Player player, Player.CardUsageParams usageParams)
    {
        throw new NotImplementedException();
    }

    public void StartOfNewTurn(Game game, Type stateType)
    {
        throw new NotImplementedException();
    }
}
