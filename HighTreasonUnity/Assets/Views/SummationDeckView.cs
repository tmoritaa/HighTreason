using System;
using System.Collections.Generic;
using System.Linq;

using HighTreasonGame;

public class SummationDeckView : CardHolderView
{
    private Player playerToDisplay;

    public void SetPlayerForDisplay(Player player)
    {
        playerToDisplay = player;
    }

    protected override CardHolder RetrieveCardHolder()
    {
        return playerToDisplay.SummationDeck;
    }
}
