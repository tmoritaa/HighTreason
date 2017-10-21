using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using HighTreasonGame;

public class BoardObjectElementManager : MonoBehaviour
{
    private static BoardObjectElementManager instance;

    public static BoardObjectElementManager Instance
    {
        get { return instance; }
    }

    private GameState.GameStateType curStateShown;

    private Dictionary<BoardObject, BoardObjectElement> boardObjToElementMap = new Dictionary<BoardObject, BoardObjectElement>();

    void Awake()
    {
        BoardObjectElementManager.instance = this;
    }

    public void MarkAllAsUnselectable()
    {
        foreach (ISelectable selectable in boardObjToElementMap.Values)
        {
            selectable.SetSelectable(false);
        }
    }

    public void MarkChoicesAsSelectable(List<BoardObject> choices)
    {
        MarkAllAsUnselectable();

        foreach (BoardObject choice in choices)
        {
            boardObjToElementMap[choice].SetSelectable(true);
        }
    }

    public void RegisterBoardElement(BoardObjectElement boe)
    {
        boardObjToElementMap.Add(boe.BoardObject, boe);
    }

    public void HandleStateChange()
    {
        bool handle = GameManager.Instance.Game.CurState.StateType == GameState.GameStateType.JurySelection
            || (curStateShown == GameState.GameStateType.JurySelection && GameManager.Instance.Game.CurState.StateType == GameState.GameStateType.TrialInChief);

        if (handle)
        {
            boardObjToElementMap.Clear();
            curStateShown = GameManager.Instance.Game.CurState.StateType;
        }
    }

}
