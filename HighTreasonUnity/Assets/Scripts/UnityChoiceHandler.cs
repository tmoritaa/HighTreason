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
        ChooseBoardObjects,
        MomentOfInsight,
        ChooseCards,
        ChooseCardEffect,
        ChooseAttorney,
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

    public override void ChoosePlayerAction(List<Card> cards, Game game, Player choosingPlayer, out Player.PlayerActionParams outPlayerAction)
    {
        ChoiceHandlerDelegator.Instance.TriggerChoice(this, choosingPlayer, new CardAndUsageInputHandler());
        waitForInput.WaitOne();

        outPlayerAction = new Player.PlayerActionParams();

        if (passedParams == null)
        {
            outPlayerAction.usage = Player.PlayerActionParams.UsageType.Cancelled;
        }
        else
        {
            outPlayerAction.usage = (Player.PlayerActionParams.UsageType)passedParams[0];

            if (outPlayerAction.usage != Player.PlayerActionParams.UsageType.Mulligan)
            {
                outPlayerAction.card = (Card)passedParams[1];

                for (int i = 2; i < passedParams.Length; ++i)
                {
                    outPlayerAction.misc.Add(passedParams[i]);
                }
            }
        }

        passedParams = null;
    }

    public override void ChooseBoardObjects(List<BoardObject> choices, 
        Func<Dictionary<BoardObject, int>, bool> validateChoices, 
        Func<List<BoardObject>, Dictionary<BoardObject, int>, 
            List<BoardObject>> filterChoices, 
        Func<Dictionary<BoardObject, int>, bool> choicesComplete, 
        Game game, 
        Player choosingPlayer, 
        string desc, 
        out BoardChoices boardChoice)
    {
        ChoiceHandlerDelegator.Instance.TriggerChoice(this, choosingPlayer, new PickBoardObjectInputHandler(desc, choices, validateChoices, filterChoices, choicesComplete));
        waitForInput.WaitOne();

        boardChoice = new BoardChoices();

        if (passedParams == null)
        {
            boardChoice.NotCancelled = false;
        }
        else
        {
            boardChoice.SelectedObjs = (Dictionary<BoardObject, int>)passedParams[0];
        }

        passedParams = null;
    }

    public override bool ChooseMomentOfInsightUse(Game game, Player choosingPlayer, out BoardChoices.MomentOfInsightInfo outMoIInfo)
    {
        ChoiceHandlerDelegator.Instance.TriggerChoice(this, choosingPlayer, new MoIInputHandler(choosingPlayer));
        waitForInput.WaitOne();

        outMoIInfo = new BoardChoices.MomentOfInsightInfo();

        bool notCancelled = true;
        if (passedParams == null)
        {
            notCancelled = false;
        }
        else if (passedParams.Length == 0)
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

        return notCancelled;
    }

    public override void ChooseCards(List<Card> choices, 
        Func<Dictionary<Card, int>, bool> validateChoices, 
        Func<List<Card>, Dictionary<Card, int>, 
            List<Card>> filterChoices, Func<Dictionary<Card, int>, bool, bool> choicesComplete, 
        bool stoppable, 
        Game game,
        Player choosingPlayer,
        string description, 
        out BoardChoices boardChoice)
    {
        ChoiceHandlerDelegator.Instance.TriggerChoice(this, choosingPlayer, new ChooseCardsInputHandler(description, choices, validateChoices, filterChoices, choicesComplete, stoppable));
        waitForInput.WaitOne();

        boardChoice = new BoardChoices();

        if (passedParams == null)
        {
            boardChoice.NotCancelled = false;
        }
        else
        {
            boardChoice.SelectedCards = (Dictionary<Card, int>)passedParams[0];
        }

        passedParams = null;
    }
    
    public override void ChooseCardEffect(Card cardToPlay, Game game, Player choosingPlayer, string description, out BoardChoices.CardPlayInfo cardPlayInfo)
    {
        ChoiceHandlerDelegator.Instance.TriggerChoice(this, choosingPlayer, new ChooseCardEffectInputHandler(cardToPlay, description));
        waitForInput.WaitOne();

        cardPlayInfo = new BoardChoices.CardPlayInfo();

        if (passedParams != null)
        {
            int idx = (int)passedParams[2];
            cardPlayInfo.eventIdx = idx;
        }

        passedParams = null;
    }

    public override void ChooseAttorneyForObjection(List<Card> validAttorneys, Game game, Player choosingPlayer, string description, out BoardChoices boardChoices)
    {
        ChoiceHandlerDelegator.Instance.TriggerChoice(this, choosingPlayer, new ChooseAttorneyInputHandler(validAttorneys, description));
        waitForInput.WaitOne();

        boardChoices = new BoardChoices();
        boardChoices.SelectedCards = (Dictionary<Card, int>)passedParams[0];

        passedParams = null;
    }
}