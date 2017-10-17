using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using HighTreasonGame;
using HighTreasonGame.GameStates;

public class TrackElement : BoardObjectElement 
{
    [SerializeField]
    protected List<Property> uniqueProperties;

    [SerializeField]
    protected TrackBoxElement trackBoxPrefab;

    [SerializeField]
    protected GameObject token;

    [SerializeField]
    protected GameObject boxParent;

    protected List<TrackBoxElement> trackBoxes = new List<TrackBoxElement>();

    protected Track track;

    protected override void init()
    {
        uniqueProperties.Add(Property.Track);

        BoardObject htgoElement = GameManager.Instance.Game.FindBO(
            (BoardObject htgo) => {
                bool retVal = true;
                uniqueProperties.ForEach(p => retVal &= htgo.Properties.Contains(p));
                return retVal;
            })[0];

        setupBOAndGO(htgoElement);

        track = (Track)htgoElement;

        int numBoxes = track.MaxValue - track.MinValue + 1;

        float xAnchorInc = 1.0f / numBoxes;
        for (int i = 0; i < numBoxes; ++i)
        {
            TrackBoxElement trackBox = Instantiate<TrackBoxElement>(trackBoxPrefab);

            RectTransform trans = trackBox.GetComponent<RectTransform>();
            trans.anchorMin = new Vector2(i * xAnchorInc, 0);
            trans.anchorMax = new Vector2((i + 1) * xAnchorInc, 1);

            trans.SetParent(boxParent.transform, false);

            trackBox.SetValue(i + track.MinValue);
            trackBoxes.Add(trackBox);
        }
    }

    protected override void updateUI()
    {
        token.transform.position = trackBoxes[track.Value - track.MinValue].transform.position;
    }
}
