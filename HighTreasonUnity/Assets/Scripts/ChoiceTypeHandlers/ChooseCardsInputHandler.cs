using System;
using System.Collections.Generic;
using System.Linq;

using HighTreasonGame;

public class ChooseCardsInputHandler : ChoiceTypeInputHandler
{
    private bool highlightChoices = false;

    private List<Card> choices;

    private Func<Dictionary<Card, int>, bool> validateChoices;
    private Func<List<Card>, Dictionary<Card, int>, List<Card>> filterChoices;
    private Func<Dictionary<Card, int>, bool, bool> choicesComplete;

    private Dictionary<Card, int> selected = new Dictionary<Card, int>();
    private List<Card> remainingChoices;

    public ChooseCardsInputHandler(
        string desc,
        List<Card> _choices,
        Func<Dictionary<Card, int>, bool> _validateChoices,
        Func<List<Card>, Dictionary<Card, int>, List<Card>> _filterChoices,
        Func<Dictionary<Card, int>, bool, bool> _choicesComplete,
        bool stoppable) 
        : base(UnityChoiceHandler.ChoiceType.ChooseCards, desc, stoppable)
    {
        choices = _choices;
        validateChoices = _validateChoices;
        filterChoices = _filterChoices;
        choicesComplete = _choicesComplete;

        remainingChoices = new List<Card>(choices);

        if (remainingChoices.Count != 0)
        {
            highlightChoices = true;
        }
    }

    public override bool SkipChoiceIfNoValid(out object[] validOutput)
    {
        validOutput = new object[] { };

        bool skipChoice = remainingChoices.Count == 0;
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

            if (remainingChoices.Contains(card))
            {
                if (!selected.ContainsKey(card))
                {
                    selected[card] = 0;
                }
                selected[card] += 1;

                bool valid = validateChoices(selected);
                if (!valid)
                {
                    selected[card] -= 1;
                    if (selected[card] < 0)
                    {
                        selected.Remove(card);
                    }

                    return false;
                }

                bool complete = choicesComplete(selected, false);
                remainingChoices = filterChoices(remainingChoices, selected);

                if (complete || remainingChoices.Count == 0)
                {
                    validOutput = new object[] { selected };
                    return true;
                }

                highlightChoices = true;
            }
        }
        else if (input[0].GetType() == typeof(string) && ((string)input[0]).Equals("done"))
        {
            bool complete = choicesComplete(selected, true);
            
            if (complete)
            {
                validOutput = new object[] { selected };
            }

            return complete;
        }

        return false;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (highlightChoices)
        {
            SelectableElementManager.Instance.MarkObjsAsSelectable(remainingChoices.ToArray());
            highlightChoices = false;
        }
    }
}
