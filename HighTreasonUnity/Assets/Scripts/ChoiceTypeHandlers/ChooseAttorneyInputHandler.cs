using System;
using System.Collections.Generic;
using System.Linq;

using HighTreasonGame;

public class ChooseAttorneyInputHandler : ChoiceTypeInputHandler
{
    private List<Card> cards;
    private bool highlightChoices = false;

    public ChooseAttorneyInputHandler(List<Card> _cards, string desc)
        : base(UnityChoiceHandler.ChoiceType.ChooseAttorney,  desc, true)
    {
        cards = _cards;

        if (cards.Count != 0)
        {
            highlightChoices = true;
        }
    }

    public override bool SkipChoiceIfNoValid(out object[] validOutput)
    {
        validOutput = new object[] { };

        bool skipChoice = cards.Count == 0;
        if (skipChoice)
        {
            validOutput = new object[] { new Dictionary<BoardObject, int>() };
        }

        return skipChoice;
    }

    public override bool VerifyInput(out object[] validOutput, params object[] input)
    {
        validOutput = new object[] { };

        if (input[0].GetType() == typeof(Card))
        {
            Card card = (Card)input[0];

            validOutput = new object[] { new Dictionary<Card, int>() { { card, 1 } } };
        }
        else if (input[0].GetType() == typeof(string) && ((string)input[0]).Equals("done"))
        {
            validOutput = new object[] { new Dictionary<Card, int>() };
        }

        return true;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (highlightChoices)
        {
            SelectableElementManager.Instance.MarkObjsAsSelectable(cards.ToArray());
            highlightChoices = false;
        }
    }
}
