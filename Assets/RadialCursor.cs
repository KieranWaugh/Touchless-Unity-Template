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

    private void Awake()
    {
       
        cursorType = CursorType.Radial;
        InteractionPoint = pinchpoint;
        

        
    }
    void Start()
    {
        cursorManager.cursors.Add(gameObject);
        print(cursorManager.cursors);
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
        gameObject.transform.localPosition = positions.getTrackedPosition(Settings.tracked_point);
        if (!pinching && Settings.pointing_method != InteractionType.Debug)
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
        return Mathf.Lerp(Settings.pinch_distance + 25, Settings.pinch_distance, Mathf.InverseLerp(Settings.pinch_distance + 20, Settings.pinch_distance, frame.GetHand(Settings.tracked_hand).PinchDistance));
    }
}
