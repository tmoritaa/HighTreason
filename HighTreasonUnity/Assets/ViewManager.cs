using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using HighTreasonGame;

using UnityEngine;

public class ViewManager : MonoBehaviour 
{
    private static ViewManager instance;

    public static ViewManager Instance
    {
        get { return instance; }
    }

    [SerializeField]
    private DetailedCardView detailedCardView;

    [SerializeField]
    private DiscardView discardView;

    [SerializeField]
    private GameObject jurySelectionMainBoardGO;

    [SerializeField]
    private GameObject trialAndSummationMainBoardGO;

    private Dictionary<BoardObject, BoardObjectElement> boardObjToElementMap = new Dictionary<BoardObject, BoardObjectElement>();

    void Awake()
    {
        ViewManager.instance = this;

        EventDelegator.Instance.NotifyStateStart += handleNotifyStateStart;
    }

    void Start()
    {
        HideAllFullscreenViews();
    }

    public void handleNotifyStateStart()
    {
        if (GameManager.Instance.Game.CurState.StateType == HighTreasonGame.GameState.GameStateType.JurySelection)
        {
            boardObjToElementMap.Clear();
            jurySelectionMainBoardGO.SetActive(true);
        }
        else if (GameManager.Instance.Game.CurState.StateType == HighTreasonGame.GameState.GameStateType.TrialInChief)
        {
            boardObjToElementMap.Clear();
            jurySelectionMainBoardGO.SetActive(false);
            trialAndSummationMainBoardGO.SetActive(true);
        }
    }

    public void DisplayDetailedCardViewWithCard(Card card)
    {
        detailedCardView.gameObject.SetActive(true);
        detailedCardView.SetCardForDisplay(card);
    }

    public void DisplayDiscardView()
    {
        discardView.gameObject.SetActive(true);
    }

    public void HideAllFullscreenViews()
    {
        HideDetailedCardView();
        HideDiscardView();
    }

    public void HideDetailedCardView()
    {
        detailedCardView.gameObject.SetActive(false);
    }

    public void HideDiscardView()
    {
        discardView.gameObject.SetActive(false);
    }

    public void UnhighlightAll()
    {
        // First unhighlight everything.
        foreach (IHighlightable highlightable in boardObjToElementMap.Values)
        {
            highlightable.Highlight(false);
        }
    }

    public void HighlightChoices(List<BoardObject> choices)
    {
        UnhighlightAll();

        foreach (BoardObject choice in choices)
        {
            boardObjToElementMap[choice].Highlight(true);
        }
    }

    public void RegisterBoardElement(BoardObjectElement boe)
    {
        boardObjToElementMap.Add(boe.BoardObject, boe);
    }
}
