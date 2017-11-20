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
            return (BoardObject)ObjRef;
        }
    }
    
    protected override void onClick()
    {
        Debug.Log("BoardObject Choice Complete");
        ChoiceHandlerDelegator.Instance.ChoiceMade(BoardObject);
    }
}
