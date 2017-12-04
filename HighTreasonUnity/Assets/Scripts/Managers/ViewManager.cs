using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using HighTreasonGame;

using UnityEngine;
using UnityEngine.UI;

public class ViewManager : MonoBehaviour 
{
    public enum PopupType
    {
        DetailedCard,
        SummationDeck,
        Discard,
        GameResult,
    };

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
    private GameResultView gameResultView;

    [SerializeField]
    private GameObject jurySelectionMainBoardGO;

    [SerializeField]
    private GameObject trialAndSummationMainBoardGO;

    [SerializeField]
    private TextHolderElement curActionDesc;

    private Stack<GameObject> displayedPopups = new Stack<GameObject>();

    void Awake()
    {
        ViewManager.instance = this;

        EventDelegator.Instance.NotifyStateStart += handleNotifyStateStart;
    }

    void Start()
    {
        detailedCardView.gameObject.SetActive(false);
        summationView.gameObject.SetActive(false);
        discardView.gameObject.SetActive(false);
        this.curActionDesc.gameObject.SetActive(false);
    }

    public void DisplaySummationDeckViewForCurPlayer()
    {
        DisplayView(PopupType.SummationDeck, GameManager.Instance.Game.CurPlayer);
    }

    public void DisplayDiscardView()
    {
        DisplayView(PopupType.Discard);
    }

    public void DisplayView(PopupType ptype, params object[] args)
    {
        GameObject displayedView = null;

        switch (ptype)
        {
            case PopupType.DetailedCard:
                detailedCardView.SetCardForDisplay((Card)args[0]);
                detailedCardView.gameObject.SetActive(true);
                displayedView = detailedCardView.gameObject;
                break;
            case PopupType.SummationDeck:
                summationView.SetPlayerForDisplay((Player)args[0]);
                summationView.gameObject.SetActive(true);
                displayedView = summationView.gameObject;
                break;
            case PopupType.Discard:
                discardView.gameObject.SetActive(true);
                displayedView = discardView.gameObject;
                break;
            case PopupType.GameResult:
                gameResultView.InitInfo((Player.PlayerSide)args[0], (bool)args[1], (int)args[2]);
                gameResultView.gameObject.SetActive(true);
                displayedView = gameResultView.gameObject;
                break;
        }

        if (displayedView != null)
        {
            displayedPopups.Push(displayedView);
        }
    }
    
    public void HideAllViews()
    {
        while (displayedPopups.Count > 0)
        {
            GameObject go = displayedPopups.Pop();
            go.SetActive(false);
        }
    }

    public void HideTopView()
    {
        displayedPopups.Pop().SetActive(false);
    }

    public void HideCurActionText()
    {
        curActionDesc.gameObject.SetActive(false);
    }

    public void UpdateActionText(string desc)
    {
        curActionDesc.SetText(desc);
        curActionDesc.gameObject.SetActive(true);
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
