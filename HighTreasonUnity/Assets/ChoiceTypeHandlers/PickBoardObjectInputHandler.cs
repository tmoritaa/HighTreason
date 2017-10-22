using System;
using System.Collections.Generic;
using System.Linq;

using HighTreasonGame;

public class PickBoardObjectInputHandler : ChoiceTypeInputHandler
{
    private bool highlightChoices = false;

    private List<BoardObject> choices;

    Func<Dictionary<BoardObject, int>, bool> validateChoices;
    Func<List<BoardObject>, Dictionary<BoardObject, int>, List<BoardObject>> filterChoices;
    Func<Dictionary<BoardObject, int>, bool> choicesComplete;

    Dictionary<BoardObject, int> selected = new Dictionary<BoardObject, int>();
    List<BoardObject> remainingChoices;

    public PickBoardObjectInputHandler(object[] _additionalParams) : base(_additionalParams)
    {
        choices = (List<BoardObject>)additionalParams[0];
        validateChoices = (Func<Dictionary<BoardObject, int>, bool>)additionalParams[1];
        filterChoices = (Func<List<BoardObject>, Dictionary<BoardObject, int>, List<BoardObject>> )additionalParams[2];
        choicesComplete = (Func<Dictionary<BoardObject, int>, bool>)additionalParams[3];

        remainingChoices = new List<BoardObject>(choices);

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

        BoardObject obj = (BoardObject)input[0];
        if (remainingChoices.Contains(obj))
        {
            if (!selected.ContainsKey(obj))
            {
                selected[obj] = 0;
            }
            selected[obj] += 1;

            bool valid = validateChoices(selected);
            if (!valid)
            {
                selected[obj] -= 1;
                if (selected[obj] < 0)
                {
                    selected.Remove(obj);
                }

                return false;
            }

            bool complete = choicesComplete(selected);
            remainingChoices = filterChoices(remainingChoices, selected);

            if (complete || remainingChoices.Count == 0)
            {
                validOutput = new object[] { selected };
                return true;
            }

            highlightChoices = true;
        }

        return false;
    }

    public override void OnUpdate()
    {
        if (highlightChoices)
        {
            BoardObjectElementManager.Instance.MarkChoicesAsSelectable(remainingChoices);
            highlightChoices = false;
        }
    }
}
