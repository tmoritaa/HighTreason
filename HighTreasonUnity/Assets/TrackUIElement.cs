using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using HighTreasonGame;

public class TrackUIElement : HTGOUIElement 
{
    [SerializeField]
    protected List<Property> uniqueProperties;

    [SerializeField]
    protected TrackBoxUIElement trackBoxPrefab;

    [SerializeField]
    protected GameObject token;

    [SerializeField]
    protected GameObject boxParent;

    protected List<TrackBoxUIElement> trackBoxes = new List<TrackBoxUIElement>();

    protected Track track;

    protected override void Awake()
	{
        properties.Add(Property.Track);
        uniqueProperties.ForEach(p => properties.Add(p));

        base.Awake();
	}

    protected virtual void Update()
    {
        token.transform.position = trackBoxes[track.Value - track.MinValue].transform.position;
    }

    protected override void initUIElement()
    {
        int numBoxes = track.MaxValue - track.MinValue + 1;

        float xAnchorInc = 1.0f / numBoxes;
        for (int i = 0; i < numBoxes; ++i)
        {
            TrackBoxUIElement trackBox = Instantiate<TrackBoxUIElement>(trackBoxPrefab);

            RectTransform trans = trackBox.GetComponent<RectTransform>();
            trans.anchorMin = new Vector2(i * xAnchorInc, 0);
            trans.anchorMax = new Vector2((i + 1) * xAnchorInc, 1);

            trans.SetParent(boxParent.transform, false);

            trackBox.SetValue(i + track.MinValue);
            trackBoxes.Add(trackBox);
        }
    }

    protected override void setHTGOElement(HTGameObject htgo)
    {
        track = (Track)htgo;
    }
}
