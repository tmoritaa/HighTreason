using System;
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
        MomentOfInsight,
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

    public override void ChoosePlayerAction(List<Card> cards, Game game, out Player.PlayerActionParams outPlayerAction)
    {
        ChoiceHandlerDelegator.Instance.TriggerChoice(this, new CardAndUsageInputHandler());
        waitForInput.WaitOne();

        outPlayerAction = new Player.PlayerActionParams();
        outPlayerAction.usage = (Player.PlayerActionParams.UsageType)passedParams[0];

        if (outPlayerAction.usage != Player.PlayerActionParams.UsageType.Mulligan)
        {
            outPlayerAction.card = (Card)passedParams[1];

            for (int i = 2; i < passedParams.Length; ++i)
            {
                outPlayerAction.misc.Add(passedParams[i]);
            }
        }

        passedParams = null;
    }

    public override void ChooseBoardObjects(List<BoardObject> choices, Func<Dictionary<BoardObject, int>, bool> validateChoices, Func<List<BoardObject>, Dictionary<BoardObject, int>, List<BoardObject>> filterChoices, Func<Dictionary<BoardObject, int>, bool> choicesComplete, Game game, out BoardChoices boardChoice)
    {
        ChoiceHandlerDelegator.Instance.TriggerChoice(this, new PickBoardObjectInputHandler(choices, validateChoices, filterChoices, choicesComplete));
        waitForInput.WaitOne();

        boardChoice = new BoardChoices();
        boardChoice.SelectedObjs = (Dictionary<BoardObject, int>)passedParams[0];

        passedParams = null;
    }

    public override bool ChooseMomentOfInsightUse(Game game, out BoardChoices.MomentOfInsightInfo outMoIInfo)
    {
        ChoiceHandlerDelegator.Instance.TriggerChoice(this, new MoIInputHandler());
        waitForInput.WaitOne();

        outMoIInfo = new BoardChoices.MomentOfInsightInfo();

        if (passedParams.Length == 0)
        {
            outMoIInfo.Use = BoardChoices.MomentOfInsightInfo.MomentOfInsightUse.NotChosen;
        }
        else
        {
            outMoIInfo.Use = (BoardChoices.MomentOfInsightInfo.MomentOfInsightUse)passedParams[0];

            if (outMoIInfo.Use == BoardChoices.MomentOfInsightInfo.MomentOfInsightUse.Swap)
            {
                outMoIInfo.HandCard = (Card)passedParams[1];
                outMoIInfo.SummationCard = (Card)passedParams[2];
            }
        }

        passedParams = null;

        return true;
    }
}