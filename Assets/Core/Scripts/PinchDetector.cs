/******************************************************************************
 * Copyright (C) Ultraleap, Inc. 2011-2023.                                   *
 *                                                                            *
 * Use subject to the terms of the Apache License 2.0 available at            *
 * http://www.apache.org/licenses/LICENSE-2.0, or another agreement           *
 * between Ultraleap and you, your company or other organization.             *
 ******************************************************************************/

using Leap;
using Leap.Unity;
using System;
using UnityEngine;


/// <summary>
/// A lightweight Pinch Detector, that calculates a pinch value based on the distance between the 
/// index tip and the thumb tip. Utilises hysteresis in order to have different pinch and unpinch thresholds.
/// </summary>
public class PinchDetector : GestureDetector
{

    [Header("Pinch Activation Settings")]
    [Tooltip("The distance between index and thumb at which to enter the pinching state.")]
    public float activateDistance = Settings.pinch_distance;
    [Tooltip("The distance between index and thumb at which to leave the pinching state.")]
    public float deactivateDistance = 0.04f;

    

    
    /// <summary>
    /// The percent value (0-1) between the activate distance and absolute pinch.
    /// Note that it is virtually impossible for the hand to be completely pinched.
    /// </summary>
    public float SquishPercent { get; private set; }

    private Chirality _chiralityLastFrame;

   

    public override void UpdateStatus(Hand hand)
    {
        if (hand == null)
        {
            return;
        }

        // Convert from mm to m
        float pinchDistance = hand.PinchDistance * 0.001f;
        //Debug.Log(pinchDistance);

        if (pinchDistance < Settings.pinch_distance)
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
        else if (pinchDistance > deactivateDistance)
        {
            if (IsGesturing)
            {
                OnUnGesture?.Invoke(hand);
            }
            IsGesturing = false;
        }

        if (IsGesturing)
        {
            SquishPercent = Mathf.InverseLerp(activateDistance, 0, pinchDistance);
        }
        else
        {
            SquishPercent = 0;
        }
    }
}