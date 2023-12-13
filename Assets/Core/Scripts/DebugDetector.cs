using System.Collections;
using System.Collections.Generic;
using Leap;
using UnityEngine;

public class DebugDetector : GestureDetector
{
    public override void UpdateStatus(Hand hand)
    {
        if (Input.GetMouseButton(0))
        {
            
            if (!IsGesturing)
            {
                
                OnGesture?.Invoke(hand);
                
            }
            else
            {
                OnHeld?.Invoke(hand);
            }

            IsGesturing = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (IsGesturing)
            {
                OnUnGesture?.Invoke(hand);
            }
            IsGesturing = false;
        }
    }
}
