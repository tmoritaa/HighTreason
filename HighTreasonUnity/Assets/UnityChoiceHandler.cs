﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Text;

using HighTreasonGame;

using UnityEngine;

public class UnityChoiceHandler : ChoiceHandler
{
    public enum ChoiceType
    {
        NoChoice,
        CardAndUsage,
        PickBoardObject,
    }

    private AutoResetEvent waitForInput = new AutoResetEvent(false);

    object[] passedParams;

    public UnityChoiceHandler()
        : base(Player.PlayerType.Human)
    {}

    public void ChoiceInputMade(object[] _passedParams)
    {
        passedParams = _passedParams;
        waitForInput.Set();
    }

    public override void ChooseCardAndUsage(List<Card> cards, Game game, out Player.CardUsageParams outCardUsage)
    {
        ChoiceHandlerDelegator.Instance.TriggerChoice(this, ChoiceType.CardAndUsage);
        waitForInput.WaitOne();

        outCardUsage = new Player.CardUsageParams();
        outCardUsage.card = (Card)passedParams[0];
        outCardUsage.usage = (Player.CardUsageParams.UsageType)passedParams[1];
        
        for (int i = 2; i < passedParams.Length; ++i)
        {
            outCardUsage.misc.Add(passedParams[i]);
        }

        passedParams = null;
    }

    public override void ChooseBoardObjects(List<BoardObject> choices, Func<Dictionary<BoardObject, int>, bool> validateChoices, Func<List<BoardObject>, Dictionary<BoardObject, int>, List<BoardObject>> filterChoices, Func<Dictionary<BoardObject, int>, bool> choicesComplete, Game game, out BoardChoices boardChoice)
    {
        ChoiceHandlerDelegator.Instance.TriggerChoice(this, ChoiceType.PickBoardObject, choices, validateChoices, filterChoices, choicesComplete);
        waitForInput.WaitOne();

        boardChoice = new BoardChoices();
        boardChoice.SelectedObjs = (Dictionary<BoardObject, int>)passedParams[0];

        passedParams = null;
    }

    public override bool ChooseMomentOfInsightUse(Game game, out BoardChoices.MomentOfInsightInfo outMoIInfo)
    {
        throw new NotImplementedException();
    }
}