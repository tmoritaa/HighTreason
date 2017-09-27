using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using HighTreasonGame;

using UnityEngine;

public class ViewManager : MonoBehaviour 
{
    private static ViewManager instance;

    public static ViewManager Instance
    {
        get { return instance; }
    }

    [SerializeField]
    private DetailedCardView detailedCardView;

    void Awake()
    {
        ViewManager.instance = this;
    }

    void Start()
    {
        detailedCardView.gameObject.SetActive(false);
    }

    public void DisplayDetailedCardViewWithCard(CardTemplate card)
    {
        detailedCardView.gameObject.SetActive(true);
        detailedCardView.SetCardForDisplay(card);
    }

    public void HideDetailedCardView()
    {
        detailedCardView.gameObject.SetActive(false);
    }
}
