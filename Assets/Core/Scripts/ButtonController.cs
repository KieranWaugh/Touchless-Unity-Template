using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ButtonController : Widget
{

    public UnityEvent onPress ;
    public Action<GameObject> buttonPress;
    

    public override void activate(bool down, bool up, bool held, bool first)
    {
        if (down || held)
        {
            if (Settings.enable_widget_click_feedback)
            {
                gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width * 0.9f);
                gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height * 0.9f);
            }
            
            

        }
        else if (up)
        {
            onPress?.Invoke();

            active = true;
            OnActivate?.Invoke(gameObject);
            OnUnActivate?.Invoke(gameObject);
            active = false;
            gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        }
        
    }

    public void SendClick(GameObject button){
        buttonPress.Invoke(button);
    }

   

    public override void setFocus(bool flag)
    {
        if (flag){
            focussed = true;
            OnFocus?.Invoke(gameObject);
            if (Settings.enable_widget_outline)
            {
                GetComponentInChildren<Outline>().enabled = true;
            }
        }
        else
        {
            focussed = false;
            OnUnFocus?.Invoke(gameObject);
            GetComponentInChildren<Outline>().enabled = false;
        }


    }

    public override float getCursorDistance()
    {
        if (!Settings.enable_proxemics)
        {
            if (cursor.transform.localPosition.x >= transform.localPosition.x - width / 2 && cursor.transform.localPosition.x <= transform.localPosition.x + width / 2 &&
                cursor.transform.localPosition.y >= transform.localPosition.y - height / 2 && cursor.transform.localPosition.y <= transform.localPosition.y + height / 2)
            {
                return -2;

            }
            else
            {
                return -1;
            }
        }
        else
        {
            return Vector3.Distance(cursor.transform.position, gameObject.transform.position);
        }

        
    }

    public override void setTarget(bool flag)
    {
        if (flag){
            target = true;
            GetComponentInChildren<Image>().color = Color.green;
        }
        else
        {
            target = false;
            GetComponentInChildren<Image>().color = Color;
        }
    }

    public override void SetColour(Color color)
    {
        GetComponentInChildren<Image>().color = color;
    }
}


