using System.Collections;
using System.Collections.Generic;
using Leap;
using UnityEngine;

public class DwellDetector : GestureDetector
{
[SerializeField] InteractionManager interactionManager;

    public override void UpdateStatus(Hand hand)
    {
        if(interactionManager.focusedWidget != null){
            if(!IsGesturing){
                
            }
        }
    }
}
