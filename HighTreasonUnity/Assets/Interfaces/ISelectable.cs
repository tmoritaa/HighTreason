﻿using System;
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
}

public static class SelectableExtension
{
    public static void SetSelectable(this ISelectable self, bool b)
    {
        self.Selectable = b;
        ((MonoBehaviour)self).GetComponent<Graphic>().raycastTarget = b;
    }
}