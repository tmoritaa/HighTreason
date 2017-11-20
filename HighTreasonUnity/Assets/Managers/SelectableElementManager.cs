using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using HighTreasonGame;

public class SelectableElementManager : MonoBehaviour
{
    private static SelectableElementManager instance;

    public static SelectableElementManager Instance
    {
        get { return instance; }
    }

    private Dictionary<object, SelectableElement> objToSelectableMap = new Dictionary<object, SelectableElement>();

    void Awake()
    {
        SelectableElementManager.instance = this;
    }

    public void MarkAllAsUnselectable()
    {
        foreach (ISelectable selectable in objToSelectableMap.Values)
        {
            selectable.SetSelectable(false);
        }
    }

    public void MarkObjsAsSelectable(object[] choices)
    {
        MarkAllAsUnselectable();

        foreach (object choice in choices)
        {
            objToSelectableMap[choice].SetSelectable(true);
        }
    }

    public void RegisterSelectableElement(SelectableElement se)
    {
        objToSelectableMap.Add(se.ObjRef, se);
    }

    public void Reset()
    {
        objToSelectableMap.Clear();
    }
}
