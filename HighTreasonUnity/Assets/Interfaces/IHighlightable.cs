using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public interface IHighlightable
{
    GameObject highlightGO { get; set; }
}

public static class HighlightableExtensions
{
    public static void Highlight(this IHighlightable self, bool b)
    {
        self.highlightGO.gameObject.SetActive(b);
    }
}
