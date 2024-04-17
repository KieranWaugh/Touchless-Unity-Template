using System.Collections;
using System.Collections.Generic;
using Leap;
using Leap.Unity;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class RadialCursor : Cursor
{
    [SerializeField] GameObject ring;
    [SerializeField] GameObject pinchpoint;
    private bool pinching = false;

    private void Start()
    {
        cursorType = CursorType.Radial;
    }
    public override void activateGesature()
    
    {
        pinching = true;
        
    }

    public override void deactivateGesature()
    {
        pinching = false;
    }

    public override void updatePositions(screenHand positions, Frame frame)
    {
        print("here");
        gameObject.transform.localPosition = positions.getTrackedPosition(Settings.tracked_point);
        if (!pinching)
        {
            ring.GetComponent<RectTransform>().sizeDelta = new Vector2(updateRing(frame), updateRing(frame));
        }
        
        //print(updateRing(frame));
        //print(frame.GetHand(Settings.tracked_hand).PinchDistance);

        //print(Mathf.InverseLerp(60f, 30f, frame.GetHand(Settings.tracked_hand).PinchDistance));
        
    }

    private float updateRing(Frame frame)
    {
        //print(frame.GetHand(Settings.tracked_hand).PinchStrength);
        return Mathf.Lerp(75, 25, Mathf.InverseLerp(Settings.pinch_distance + 20, Settings.pinch_distance, frame.GetHand(Settings.tracked_hand).PinchDistance));
    }
}
