using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using HighTreasonGame;

public class GameManager : MonoBehaviour 
{
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

        Game = new Game(new ChoiceHandler[] { new UnityChoiceHandler(), new UnityChoiceHandler() }, cardInfoTxt.text);
    }

    void Start()
    {
        Game.NotifyGameEnd += onGameEnd;

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
        Debug.Log("Player " + winningPlayer.ToString() + " won with score " + finalScore + " notEnoughGuiltVictory=" + notEnoughGuiltVictory.ToString());
    }
}
