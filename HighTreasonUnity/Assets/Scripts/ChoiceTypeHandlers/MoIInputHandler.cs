using System;
using System.Collections.Generic;
using System.Linq;

using HighTreasonGame;

public class MoIInputHandler : ChoiceTypeInputHandler
{
    private bool highlightChoices = false;

    private List<Card> handChoices;
    private List<Card> summationChoices;

    private Card pickedHandCard = null;
    private Card pickedSummationCard = null;

    private Player choosingPlayer;

    public MoIInputHandler(Player _choosingPlayer) : base(UnityChoiceHandler.ChoiceType.MomentOfInsight, "Select Moment of Insight use", false)
    {
        choosingPlayer = _choosingPlayer;

        if (canSwap())
        {
            handChoices = choosingPlayer.Hand.Cards.Where(c => !c.BeingPlayed).ToList();
            summationChoices = choosingPlayer.SummationDeck.Cards;
        }

        highlightChoices = canSwap() || canReveal();
    }

    public override bool SkipChoiceIfNoValid(out object[] validOutput)
    {
        validOutput = new object[] {};
        
        return !canSwap() && !canReveal();
    }

    private bool canSwap()
    {
        return choosingPlayer.SummationDeck.Cards.Count != 0;
    }

    private bool canReveal()
    {
        return GameManager.Instance.Game.GetOtherPlayer(choosingPlayer).SummationDeck.Cards.Count != 0;
    }

    public override bool VerifyInput(out object[] validOutput, params object[] input)
    {
        bool complete = false;

        validOutput = new object[0];

        BoardChoices.MomentOfInsightInfo.MomentOfInsightUse usage = (BoardChoices.MomentOfInsightInfo.MomentOfInsightUse)input[0];

        if (usage == BoardChoices.MomentOfInsightInfo.MomentOfInsightUse.Swap)
        {
            Card card = (Card)input[1];

            if (card.CardHolder.Id == CardHolder.HolderId.Hand)
            {
                pickedHandCard = card;
            }
            else if (card.CardHolder.Id == CardHolder.HolderId.Summation)
            {
                pickedSummationCard = card;
            }

            if (pickedHandCard != null && pickedSummationCard != null)
            {
                complete = true;
            }

            validOutput = new object[] { usage, pickedHandCard, pickedSummationCard };
        }
        else if (usage == BoardChoices.MomentOfInsightInfo.MomentOfInsightUse.Reveal)
        {
            complete = true;
            validOutput = new object[] { usage };
        }

        if (!complete)
        {
            highlightChoices = true;
        }

        return complete;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (highlightChoices)
        {
            List<object> objs = new List<object>();

            if (canSwap())
            {
                if (pickedHandCard == null)
                {
                    objs.AddRange(handChoices.Cast<object>().ToArray());
                }

                if (pickedSummationCard == null)
                {
                    objs.AddRange(summationChoices.Cast<object>().ToArray());
                }
            }
            
            if (objs.Count > 0)
            {
                SelectableElementManager.Instance.MarkObjsAsSelectable(objs.ToArray());
            }

            highlightChoices = false;
        }
    }
}
