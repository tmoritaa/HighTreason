using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

public class TrackBoxUIElement : MonoBehaviour 
{
    [SerializeField]
    private Text text;

    private int value;

	void Start()
	{
		
	}
	
	void Update()
	{
		
	}

    public void SetValue(int i)
    {
        value = i;
        text.text = value.ToString();
    }
}
