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
    private SummationDeckView summationView;

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

    public void DisplaySummationDeckView()
    {
        summationView.gameObject.SetActive(true);
    }

    public void HideAllFullscreenViews()
    {
        HideDetailedCardView();
        HideDiscardView();
        HideSummationDeckView();
    }

    public void HideDetailedCardView()
    {
        detailedCardView.gameObject.SetActive(false);
    }

    public void HideDiscardView()
    {
        discardView.gameObject.SetActive(false);
    }

    public void HideSummationDeckView()
    {
        summationView.gameObject.SetActive(false);
    }

    // TODO: should probably move mark stuff to somewhere else.
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
}
