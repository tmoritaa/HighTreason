using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HighTreasonGame;

public class SummationDeckView : CardHolderView
{
    protected override CardHolder RetrieveCardHolder()
    {
        return GameManager.Instance.Game.CurPlayer.SummationDeck;
    }
}
