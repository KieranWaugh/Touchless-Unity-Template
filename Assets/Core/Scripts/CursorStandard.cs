using System.Collections;
using System.Collections.Generic;
using Leap;
using UnityEngine;

public class CursorStandard : Cursor
{
    
    public override void activateGesature()
    {
        gameObject.GetComponentInChildren<RectTransform>().sizeDelta = new Vector2(20, 20);
    }

    private void Start()
    {
        cursorType = CursorType.Standard;
        InteractionPoint = gameObject;
        cursorManager.cursors.Add(gameObject);
    }

    public override void deactivateGesature()
    {
        gameObject.GetComponentInChildren<RectTransform>().sizeDelta = new Vector2(30, 30);
    }

    public override void updatePositions(screenHand positions, Frame frame)
    {
        var position = positions.getTrackedPosition(Settings.tracked_point);


        if (Settings.pointing_method == InteractionType.Debug)
        {
            if (interactionManager.settings.activeInHierarchy)
            {
                gameObject.transform.localPosition = position;
               
            }
            else
            {
                gameObject.transform.position = position;
              
            }

        }
        //else if (Settings.pointing_method == InteractionType.ControlDisplayGain)
        //{
        //    print(position);
        //    position = Settings.gain * position;
        //    print(Settings.gain);
        //    gameObject.transform.localPosition = position;
         
        //}

        else
        {
            
            gameObject.transform.localPosition = position;
          
        }
    }
}
