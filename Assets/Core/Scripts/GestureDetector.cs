using Leap;
using Leap.Unity;
using System;
using UnityEngine;

public abstract class GestureDetector : MonoBehaviour
{
    public Action<Hand> OnGesture, OnUnGesture, OnHeld;
    protected LeapServiceProvider leapProvider;
    public bool IsGesturing{ get; set; }
    void Start()
    {
        if (leapProvider == null)
        {
            leapProvider = FindObjectOfType<LeapServiceProvider>();
        }
    
    }
    void Update()
    {
        if(Settings.pointing_method == InteractionType.Debug){
            UpdateStatus(new Hand());
        }else{
            if(leapProvider != null && leapProvider.CurrentFrame != null){
            UpdateStatus(leapProvider.CurrentFrame.GetHand(Settings.tracked_hand));   
        }
        }
        
    }

    public abstract void UpdateStatus(Hand hand);
}
