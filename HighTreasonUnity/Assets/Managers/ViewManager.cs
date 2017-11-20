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

    private GameState.GameStateType curStateShown;

    [SerializeField]
    private Color prosecutionColor;
    public Color ProsecutionColor { get { return prosecutionColor; } }

    [SerializeField]
    private Color defenseColor;
    public Color DefenseColor { get { return defenseColor; } }

    [SerializeField]
    private Color neutralColor;
    public Color NeutralColor { get { return neutralColor; } }

    [SerializeField]
    private Color jurySelectColor;
    public Color JurySelectColor { get { return jurySelectColor; } }

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

    void Awake()
    {
        ViewManager.instance = this;

        EventDelegator.Instance.NotifyStateStart += handleNotifyStateStart;
    }

    void Start()
    {
        HideAllFullscreenViews();
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

    private void handleNotifyStateStart()
    {
        bool isJurySelect = GameManager.Instance.Game.CurState.StateType == GameState.GameStateType.JurySelection;
        bool isFirstTrialInChief = (curStateShown == GameState.GameStateType.JurySelection && GameManager.Instance.Game.CurState.StateType == GameState.GameStateType.TrialInChief);

        if (isJurySelect || isFirstTrialInChief)
        {
            SelectableElementManager.Instance.Reset();
            curStateShown = GameManager.Instance.Game.CurState.StateType;
        }

        if (isJurySelect)
        {
            jurySelectionMainBoardGO.SetActive(true);
        }
        else if (isFirstTrialInChief)
        {
            jurySelectionMainBoardGO.SetActive(false);
            trialAndSummationMainBoardGO.SetActive(true);
        }
    }
}
