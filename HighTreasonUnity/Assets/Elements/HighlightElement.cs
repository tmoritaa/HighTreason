using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class HighlightElement : MonoBehaviour
{
    [SerializeField]
    protected GameObject highlightPrefab;

    protected GameObject highlightGO;

    protected virtual void Awake()
    {
        highlightGO = GameObject.Instantiate(highlightPrefab);

        highlightGO.transform.SetParent(this.transform, false);
        Highlight(false);
    }

    public void Highlight(bool b)
    {
        highlightGO.gameObject.SetActive(b);
    }
}
