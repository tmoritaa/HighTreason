using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using HighTreasonGame;

public class TrackUIElement : HTGOUIElement 
{
    [SerializeField]
    private List<Property> uniqueProperties;

    [SerializeField]
    private TrackBoxUIElement trackBoxPrefab;

    private List<TrackBoxUIElement> trackBoxes = new List<TrackBoxUIElement>();

    private Track track;

    protected override void Awake()
	{
        properties.Add(Property.Track);
        uniqueProperties.ForEach(p => properties.Add(p));

        base.Awake();
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
            trans.SetParent(this.transform, false);

            trackBox.SetValue(i + track.MinValue);
            trackBoxes.Add(trackBox);
        }
    }

    protected override void setHTGOElement(HTGameObject htgo)
    {
        track = (Track)htgo;
    }
}
