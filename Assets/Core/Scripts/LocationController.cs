using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class LocationController : MonoBehaviour
{
    [SerializeField] GameObject top;
    [SerializeField] GameObject bottom;
    [SerializeField] GameObject left;
    [SerializeField] GameObject right;

    [SerializeField] GameObject Leap;

    private RGB topVal;
    private RGB bottomVal;
    private RGB leftVal;
    private RGB rightVal;

    Vector3 pos;
    // Start is called before the first frame update

    
    void Start()
    {

        topVal = new RGB(top.GetComponent<Image>().color.r, top.GetComponent<Image>().color.g,top.GetComponent<Image>().color.b);
        bottomVal = new RGB(bottom.GetComponent<Image>().color.r, bottom.GetComponent<Image>().color.g,bottom.GetComponent<Image>().color.b);
        leftVal = new RGB(left.GetComponent<Image>().color.r, left.GetComponent<Image>().color.g, left.GetComponent<Image>().color.b);
        rightVal = new RGB(right.GetComponent<Image>().color.r, right.GetComponent<Image>().color.g, right.GetComponent<Image>().color.b);
    }
    // Update is called once per frame
    
    void Update(){

        top.GetComponent<Image>().color = new Color(topVal.R, topVal.G, topVal.B);
        bottom.GetComponent<Image>().color = new Color(bottomVal.R, bottomVal.G, bottomVal.B);
        left.GetComponent<Image>().color = new Color(leftVal.R, leftVal.G, leftVal.B);
        right.GetComponent<Image>().color = new Color(rightVal.R, rightVal.G, rightVal.B);
    }

    public void updateColour(Vector3 pos){
        //print(topVal.R);
        topVal = GetColorCode(pos.y, (float) 1, (float)0.5);
        bottomVal = GetColorCode(pos.y,(float) 0, (float)0.5);
        leftVal = GetColorCode(pos.x,(float) -0.75, 0);
        rightVal = GetColorCode(pos.x,(float) 0.75, 0);
    }

    public static RGB GetColorCode(float currentValue, float maxValue, float minValue)
    {
         var percentage = (currentValue - minValue) / (maxValue - minValue);

         // Clamp the percentage to the range [0, 1]
        percentage = Math.Max(0, Math.Min(1, percentage));
        //print(percentage);
        // Calculate the red and green components.
        // Red is higher when the percentage is higher.
        // Green is higher when the percentage is lower.
        int red = (int)(255 * percentage);
        int green = 255 - red;

        return new RGB((float)red/(float)255, (float)green/(float)255, 0);
        
        
    }

}

public class RGB
{
    public float R { get; set; }
    public float G { get; set; }
    public float B { get; set; }
    public RGB(float R, float G, float B)
    {
        this.R = R;
        this.G = G;
        this.B = B;
    }
    
}
