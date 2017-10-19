using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class JuryElementGenerator : MonoBehaviour 
{
    [SerializeField]
    private JuryElement juryElementPrefab;

    [SerializeField]
    private int juryTotal;

    [SerializeField]
    private int numRows;

    [SerializeField]
    private float xStart;

    [SerializeField]
    private float xStep;

    [SerializeField]
    private float yStart;

    [SerializeField]
    private float yStep;

	void Start()
	{
        int xTotal = juryTotal / numRows;
        int yTotal = numRows;

		for (int y = 0; y < yTotal; ++y)
        {
            for (int x = 0; x < xTotal; ++x)
            {
                Vector2 anchor = new Vector2(xStart + xStep * (x), 1f - (yStart + yStep * y));
                JuryElement newEle = GameObject.Instantiate<JuryElement>(juryElementPrefab);

                newEle.GetComponent<RectTransform>().anchorMin = anchor;
                newEle.GetComponent<RectTransform>().anchorMax = anchor;

                newEle.transform.SetParent(this.transform, false);

                newEle.Idx = y * xTotal + x;
            }
        }
	}
}
