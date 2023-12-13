using Leap;
using Leap.Unity;
using System;
using UnityEngine;

public class AirPushDetector : GestureDetector
{
    [Header("AirPush Activation Settings")]
    [Tooltip("The distance from the sensor to enter a acrtivated state in m. + dentoes towards the dislpay and - dentoes towards the user")]
    public float ActivationPosition = Settings.airpush_position;

    public override void UpdateStatus(Hand hand)
    {
        if (hand == null)
        {
            return;
        }
       
        float IndexPosition = hand.GetIndex().TipPosition.z;

        if (IndexPosition > Settings.airpush_position)
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
        else if (IndexPosition < ActivationPosition)
        {
            if (IsGesturing)
            {
                OnUnGesture?.Invoke(hand);
            }
            IsGesturing = false;
        }

        
    }
}
