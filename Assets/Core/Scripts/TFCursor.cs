using System.Collections;
using System.Collections.Generic;
using Leap;
using UnityEngine;

public class TFCursor : Cursor
{
    [SerializeField] private GameObject pinchPoint;
    [SerializeField] private GameObject index;
    [SerializeField] private GameObject thumb;

    public override void activateGesature()
    {
    }

    public override void deactivateGesature()
    {
    }

    private void Awake()
    {
        InteractionPoint = pinchPoint;
    }
    

    public override void updatePositions(screenHand positions, Frame frame)
    {
        var pos_pp = positions.PinchPoint;
        var index_pos = positions.Index;
        var thumb_pos = positions.Thumb;

        pinchPoint.transform.localPosition = pos_pp;
        index.transform.localPosition = index_pos;
        thumb.transform.localPosition = thumb_pos;

    }
}
