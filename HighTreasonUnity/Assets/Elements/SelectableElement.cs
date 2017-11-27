using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

public abstract class SelectableElement : HighlightElement, ISelectable
{
    public bool Selectable
    {
        get; set;
    }

    public object SelectKey
    {
        get; set;
    }

    protected override void Awake()
    {
        base.Awake();

        this.InitISelectable();
        this.GetComponent<Button>().onClick.AddListener(onClick);
    }

    void Start()
    {
        init();
    }

    protected override void Update()
    {
        bool selectableRes = isSelectable();
        if (Selectable != selectableRes)
        {
            this.SetSelectable(selectableRes);
        }

        base.Update();

        updateUI();
    }

    protected abstract void init();
    protected abstract void updateUI();
    protected abstract void onClick();
    protected abstract bool isSelectable();
}
