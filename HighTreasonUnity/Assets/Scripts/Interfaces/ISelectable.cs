using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

public interface ISelectable
{
    bool Selectable
    {
        get; set;
    }

    object SelectKey
    {
        get; set;
    }
}

public static class SelectableExtension
{
    public static void InitISelectable(this ISelectable self)
    {
        self.SelectKey = null;
        self.SetSelectable(false);
    }

    public static void SetSelectable(this ISelectable self, bool b)
    {
        self.Selectable = b;
        ((MonoBehaviour)self).GetComponent<Graphic>().raycastTarget = b;
    }
}
