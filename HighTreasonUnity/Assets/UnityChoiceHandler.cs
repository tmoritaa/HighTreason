using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Text;

using HighTreasonGame;

using UnityEngine;

public class UnityChoiceHandler : ChoiceHandler
{
    private ManualResetEvent waitForInput = new ManualResetEvent(false);

    object[] passedParams;

    public UnityChoiceHandler()
        : base(Player.PlayerType.Human)
    {}

    public void ChoiceInputMade(object[] _passedParams)
    {
        passedParams = _passedParams;
        waitForInput.Set();
    }

    public override void ChooseCardAndUsage(List<CardTemplate> cards, Game game, out Player.CardUsageParams outCardUsage)
    {
        ChoiceHandlerDelegator.Instance.TriggerChoice(this);
        waitForInput.WaitOne();

        outCardUsage = new Player.CardUsageParams();
        outCardUsage.usage = (Player.CardUsageParams.UsageType)passedParams[0];
        
        for (int i = 1; i < passedParams.Length; ++i)
        {
            outCardUsage.misc.Add(passedParams[i]);
        }

        passedParams = null;
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