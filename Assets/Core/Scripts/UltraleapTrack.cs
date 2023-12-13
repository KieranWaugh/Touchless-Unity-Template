using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;

public class UltraleapTrack : TouchlessInputController
{

private Frame CurrentFrame = null;
    [SerializeField] private LeapProvider leapProvider;
    void OnEnable(){
        leapProvider.OnUpdateFrame += FrameRecieved;

    }

    void OnDisable(){
        leapProvider.OnUpdateFrame -= FrameRecieved;
    }

    public override void Poll()
    {

        if(CurrentFrame != null){
            if(CurrentFrame.Hands.Count != 0){
                if(CurrentFrame.Hands[0].GetChirality() == Settings.tracked_hand){
                    PositionUpdate?.Invoke(CurrentFrame.Hands[0].PalmPosition, true);
                }

             }else{
                PositionUpdate?.Invoke(Vector3.zero, false);
             }
        }else{
            PositionUpdate?.Invoke(Vector3.zero, false);
        }

        
    }

    void FrameRecieved(Frame frame){
        CurrentFrame = frame;
    }

}
