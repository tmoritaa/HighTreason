using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Text;

using HighTreasonGame;

public class UnityChoiceHandler : ChoiceHandler
{
    private ManualResetEvent waitForInput = new ManualResetEvent(false);

    public UnityChoiceHandler()
        : base(Player.PlayerType.Human)
    {}

    public void ChoiceInputMade()
    {
        waitForInput.Set();
    }

    public override void ChooseCardAndUsage(List<CardTemplate> cards, Game game, out Player.CardUsageParams outCardUsage)
    {
        ChoiceHandlerDelegator.Instance.TriggerChoice(this);
        waitForInput.WaitOne();

        throw new NotImplementedException();
    }

    public override bool ChooseMomentOfInsightUse(Game game, out BoardChoices.MomentOfInsightInfo outMoIInfo)
    {
        throw new NotImplementedException();
    }

    public override void ChooseBoardObjects(List<BoardObject> choices, Func<Dictionary<BoardObject, int>, bool> validateChoices, Func<List<BoardObject>, Dictionary<BoardObject, int>, List<BoardObject>> filterChoices, Func<Dictionary<BoardObject, int>, bool> choicesComplete, Game game, out BoardChoices boardChoice)
    {
        throw new NotImplementedException();
    }
}