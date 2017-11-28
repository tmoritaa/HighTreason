using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public abstract class HighlightElement : MonoBehaviour
{
    protected GameObject highlightGO;

    protected virtual void Awake()
    {
        GameObject highlightPrefab = (GameObject)Resources.Load("Highlight");

        highlightGO = GameObject.Instantiate(highlightPrefab);

        highlightGO.transform.SetParent(this.transform, false);
        highlight(false);
    }

    protected virtual void Update()
    {
        if (!highlightGO.gameObject.activeInHierarchy && shouldHighlight())
        {
            highlight(true);
        }
        else if (highlightGO.gameObject.activeInHierarchy && !shouldHighlight())
        {
            highlight(false);
        }
    }

    protected abstract bool shouldHighlight();

    protected void highlight(bool b)
    {
        highlightGO.gameObject.SetActive(b);
    }
}
