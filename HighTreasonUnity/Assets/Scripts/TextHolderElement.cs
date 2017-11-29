using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

public class TextHolderElement : MonoBehaviour 
{
    private Text text;

    void Awake()
    {
        text = this.GetComponentInChildren<Text>();
        this.gameObject.SetActive(false);
    }

    public void SetText(string str)
    {
        text.text = str;
    }
}
