using System;
using System.Collections.Generic;
using System.Linq;

using HighTreasonGame;

using UnityEngine;
using UnityEngine.UI;

public abstract class BoardObjectElement : SelectableElement
{
    public BoardObject BoardObject
    {
        get
        {
            return (BoardObject)SelectKey;
        }
    }
    
    protected override void onClick()
    {
        if (isSelectable())
        {
            Debug.Log("BoardObject Choice Complete");
            ChoiceHandlerDelegator.Instance.ChoiceMade(BoardObject);
        }
    }

    protected override bool shouldHighlight()
    {
        return Selectable;
    }

    protected override bool isSelectable()
    {
        return SelectableElementManager.Instance.KeyIsSelectable(SelectKey);
    }
}
