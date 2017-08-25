using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using HighTreasonGame;
using HighTreasonGame.GameStates;

public class TrackElement : GameUIElement 
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

    public override void InitUIElement()
    {
        uniqueProperties.Add(Property.Track);

        HTGameObject htgoElement = GameManager.Instance.Game.GetHTGOFromCondition(
            (HTGameObject htgo) => {
                bool retVal = true;
                uniqueProperties.ForEach(p => retVal &= htgo.Properties.Contains(p));
                return retVal;
            })[0];

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

        base.InitUIElement();
    }

    protected override void updateUIElement()
    {
        token.transform.position = trackBoxes[track.Value - track.MinValue].transform.position;
    }
}
