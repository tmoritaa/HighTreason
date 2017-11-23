using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

public abstract class SelectableElement : HighlightElement, ISelectable
{
    public object ObjRef
    {
        get; protected set;
    }

    public bool Selectable
    {
        get; set;
    }

    protected override void Awake()
    {
        base.Awake();

        this.SetSelectable(false);

        this.GetComponent<Button>().onClick.AddListener(onClick);
    }

    void Start()
    {
        init();
    }

    void Update()
    {
        updateUI();
    }

    protected abstract void init();

    protected virtual void updateUI()
    {
        if (Selectable && !this.highlightGO.activeSelf)
        {
            this.Highlight(true);
        }
        else if (!Selectable && this.highlightGO.activeSelf)
        {
            this.Highlight(false);
        }
    }

    protected abstract void onClick();

    protected void registerObj(object obj)
    {
        ObjRef = obj;
        SelectableElementManager.Instance.RegisterSelectableElement(this);
    }

}
