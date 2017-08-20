using System;
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
        get
        {
            return instance;
        }
    }

    public delegate void initGameEvent();

    public initGameEvent notifyInitGame;

    public Game Game
    {
        get; private set;
    }

    void Awake()
    {
        GameManager.instance = this;

        Game = new Game(new UnityEventHandler(), new IChoiceHandler[] { new UnityChoiceHandler(), new UnityChoiceHandler() });
    }

    void Start()
    {
        notifyInitGame();

        Game.StartGame();
    }

    public void StartGame()
    {
        
    }
}
