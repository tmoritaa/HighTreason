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
    private GameObject jurySelectionMainBoardGO;

    [SerializeField]
    private GameObject trialAndSummationMainBoardGO;

    private List<BoardObjectElement> boardElements = new List<BoardObjectElement>();

    void Awake()
    {
        ViewManager.instance = this;

        EventDelegator.Instance.NotifyStateStart += handleNotifyStateStart;
    }

    void Start()
    {
        detailedCardView.gameObject.SetActive(false);
    }

    public void handleNotifyStateStart()
    {
        if (GameManager.Instance.Game.CurState.StateType == HighTreasonGame.GameState.GameStateType.JurySelection)
        {
            jurySelectionMainBoardGO.SetActive(true);
            
        }
        else if (GameManager.Instance.Game.CurState.StateType == HighTreasonGame.GameState.GameStateType.TrialInChief)
        {
            jurySelectionMainBoardGO.SetActive(false);
            trialAndSummationMainBoardGO.SetActive(true);
        }
    }

    public void DisplayDetailedCardViewWithCard(CardTemplate card)
    {
        detailedCardView.gameObject.SetActive(true);
        detailedCardView.SetCardForDisplay(card);
    }

    public void HideDetailedCardView()
    {
        detailedCardView.gameObject.SetActive(false);
    }

    public void HighlightChoices(List<BoardObject> choices)
    {
        foreach (BoardObject choice in choices)
        {
            BoardObjectElement boe = boardElements.Find(be => be.BoardObject == choice);
            if (boe != null)
            {
                boe.Highlight(true);
            }
        }
    }

    public void RegisterBoardElement(BoardObjectElement boe)
    {
        boardElements.Add(boe);
    }

    public void UnregisterBoardElement(BoardObjectElement boe)
    {
        boardElements.Remove(boe);
    }
}
