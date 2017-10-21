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

        Game = new Game(new ChoiceHandler[] { new UnityChoiceHandler(), new UnityChoiceHandler() });
    }

    void Start()
    {
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
}
