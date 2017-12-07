using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using HighTreasonGame;

public class GameManager : MonoBehaviour 
{
    [SerializeField]
    private bool skipToTrialInChief;

    [SerializeField]
    private bool skipToSummation;

    private static GameManager instance;

    public static GameManager Instance
    {
        get { return instance; }
    }

    public Game Game
    {
        get; private set;
    }

    private Thread gameThread;

    void Awake()
    {
        GameManager.instance = this;

        TextAsset cardInfoTxt = Resources.Load("HighTreasonCardTexts") as TextAsset;

        GameState.GameStateType startState = GameState.GameStateType.JurySelection;
        if (skipToTrialInChief)
        {
            startState = GameState.GameStateType.TrialInChief;
        }
        else if (skipToSummation)
        {
            startState = GameState.GameStateType.Summation;
        }

        Game = new Game(new ChoiceHandler[] { new UnityChoiceHandler(), new UnityChoiceHandler() }, cardInfoTxt.text, startState);
    }

    void Start()
    {
        EventDelegator.Instance.NotifyGameEnd += onGameEnd;

        gameThread = new Thread(new ThreadStart(
            () => {
                try
                {
                    Game.StartGame();
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
            }));

        gameThread.Start();
    }

    void OnDestroy()
    {
        gameThread.Abort();
    }

    private void onGameEnd(Player.PlayerSide winningPlayer, bool notEnoughGuiltVictory, int finalScore)
    {
        ViewManager.Instance.DisplayView(ViewManager.PopupType.GameResult, false, winningPlayer, notEnoughGuiltVictory, finalScore);
    }
}
