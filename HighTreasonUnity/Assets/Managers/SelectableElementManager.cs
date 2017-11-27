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

    private Dictionary<object, bool> objIsSelectableTable = new Dictionary<object, bool>();

    void Awake()
    {
        SelectableElementManager.instance = this;
    }

    public void MarkObjsAsSelectable(object[] choices)
    {
        Reset();

        foreach (object choice in choices)
        {
            objIsSelectableTable[choice] = true;
        }
    }

    public void Reset()
    {
        objIsSelectableTable.Clear();
    }

    public bool KeyIsSelectable(object key)
    {
        return objIsSelectableTable.ContainsKey(key) && objIsSelectableTable[key];
    }
}
