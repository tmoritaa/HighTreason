using System;
using System.Collections.Generic;
using System.Linq;

using HighTreasonGame;

using UnityEngine;
using UnityEngine.UI;

public abstract class BoardObjectElement : MonoBehaviour, IHighlightable
{
    public BoardObject BoardObject
    {
        get; protected set;
    }

    [SerializeField]
    protected GameObject highlightGORef;
    public GameObject highlightGO
    {
        get; set;
    }

    void Awake()
    {
        highlightGO = highlightGORef;

        this.GetComponent<Button>().onClick.AddListener(onClick);
    }

    void Start()
    {
        this.Highlight(false);

        init();
    }

    void Update()
    {
        updateUI();
    }

    protected abstract void init();
    protected abstract void updateUI();
    
    protected void onClick()
    {
        if (highlightGO.activeSelf)
        {
            Debug.Log("BoardObject Choice Complete");
            ChoiceHandlerDelegator.Instance.ChoiceMade(BoardObject);
        }
    }

    protected void setupBOAndGO(BoardObject bo)
    {
        BoardObject = bo;
        ViewManager.Instance.RegisterBoardElement(this);
    }
}
