using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

public class TrackBoxElement : MonoBehaviour 
{
    [SerializeField]
    private List<Text> numTexts;

    private int value;

    public void SetValue(int i)
    {
        value = i;

        numTexts.ForEach(t => t.text = value.ToString());
    }
}
