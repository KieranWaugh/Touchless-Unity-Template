
using UnityEngine;
using UnityEngine.UI;

public class SliderController : Widget
{
    private Vector2 initPos;
    private Vector3 lastPos;
    public float sensitivity = 100;
    

    private void MoveSlider(GameObject cursor)
    {

        
        Slider slider = gameObject.GetComponent<Slider>();
        RectTransform sliderRectTransform = slider.GetComponent<RectTransform>();
        
        

        if (!Settings.enable_proxemics)
        {

            if(slider.direction == Slider.Direction.LeftToRight){
                // Calculate the slider value based on cursor's position
                Vector2 position = new Vector2(cursor.transform.position.x, cursor.transform.position.y);
                
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(sliderRectTransform, position, null, out Vector2 localPoint))
                {
                    float normalizedValue = Mathf.InverseLerp(sliderRectTransform.rect.min.x, sliderRectTransform.rect.max.x, localPoint.x);
                    slider.value = normalizedValue;
                }
            }else if(slider.direction == Slider.Direction.RightToLeft){
                // Calculate the slider value based on cursor's position
                Vector2 position = new Vector2(cursor.transform.position.x, cursor.transform.position.y);
                
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(sliderRectTransform, position, null, out Vector2 localPoint))
                {
                    float normalizedValue = Mathf.InverseLerp(sliderRectTransform.rect.max.x, sliderRectTransform.rect.min.x, localPoint.x);
                    slider.value = normalizedValue;
                }
            }    
            else if(slider.direction == Slider.Direction.TopToBottom){
                // Calculate the slider value based on cursor's position
                Vector2 position = new Vector2(cursor.transform.position.x, cursor.transform.position.y);
                
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(sliderRectTransform, position, null, out Vector2 localPoint))
                {
                    float normalizedValue = Mathf.InverseLerp(sliderRectTransform.rect.max.y, sliderRectTransform.rect.min.y, localPoint.y);
                    slider.value = normalizedValue;
                }
            }else if(slider.direction == Slider.Direction.BottomToTop){
                // Calculate the slider value based on cursor's position
                Vector2 position = new Vector2(cursor.transform.position.x, cursor.transform.position.y);
                
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(sliderRectTransform, position, null, out Vector2 localPoint))
                {
                    float normalizedValue = Mathf.InverseLerp(sliderRectTransform.rect.min.y, sliderRectTransform.rect.max.y, localPoint.y);
                    slider.value = normalizedValue;
                }
            }
            
        }
        else
        {
            Vector2 size = RectTransformUtility.PixelAdjustRect(sliderRectTransform, transform.parent.gameObject.GetComponent<Canvas>()).size * 3;
            if(slider.direction == Slider.Direction.LeftToRight){
                var sliderLengthInPixels = size.x; // Use size.y for a vertical slider
                // Calculate the difference in mouse position since the last frame
                Vector3 delta = cursor.transform.localPosition - lastPos;
                lastPos = cursor.transform.localPosition;

                // Convert the delta to a change in the slider value
                float sliderDelta = delta.x; // Use delta.y for a vertical slider

                // Map the pixel movement to the slider's value range
                float valueDelta = sliderDelta / sliderLengthInPixels * (slider.maxValue - slider.minValue);

                // Update the slider value
                slider.value += valueDelta;

                // Optional: Clamp the slider value to its min and max values
                slider.value = Mathf.Clamp(slider.value, slider.minValue, slider.maxValue);
            }
            else if(slider.direction == Slider.Direction.RightToLeft){
                var sliderLengthInPixels = size.x; // Use size.y for a vertical slider
                // Calculate the difference in mouse position since the last frame
                Vector3 delta = cursor.transform.localPosition - lastPos;
                lastPos = cursor.transform.localPosition;

                // Convert the delta to a change in the slider value
                float sliderDelta = -delta.x; // Use delta.y for a vertical slider

                // Map the pixel movement to the slider's value range
                float valueDelta = sliderDelta / sliderLengthInPixels * (slider.maxValue - slider.minValue);

                // Update the slider value
                slider.value += valueDelta;

                // Optional: Clamp the slider value to its min and max values
                slider.value = Mathf.Clamp(slider.value, slider.minValue, slider.maxValue); 
            }
            else if(slider.direction == Slider.Direction.TopToBottom ){
                var sliderLengthInPixels = size.y; // Use size.y for a vertical slider
                // Calculate the difference in mouse position since the last frame
                Vector3 delta = cursor.transform.localPosition - lastPos;
                lastPos = cursor.transform.localPosition;

                // Convert the delta to a change in the slider value
                float sliderDelta = -delta.y; // Use delta.y for a vertical slider

                // Map the pixel movement to the slider's value range
                float valueDelta = sliderDelta / sliderLengthInPixels * (slider.maxValue - slider.minValue);

                // Update the slider value
                slider.value += valueDelta;

                // Optional: Clamp the slider value to its min and max values
                slider.value = Mathf.Clamp(slider.value, slider.minValue, slider.maxValue);
            }else if(slider.direction == Slider.Direction.BottomToTop){
                var sliderLengthInPixels = size.y; // Use size.y for a vertical slider
                // Calculate the difference in mouse position since the last frame
                Vector3 delta = cursor.transform.localPosition - lastPos;
                lastPos = cursor.transform.localPosition;

                // Convert the delta to a change in the slider value
                float sliderDelta = delta.y; // Use delta.y for a vertical slider

                // Map the pixel movement to the slider's value range
                float valueDelta = sliderDelta / sliderLengthInPixels * (slider.maxValue - slider.minValue);

                // Update the slider value
                slider.value += valueDelta;

                // Optional: Clamp the slider value to its min and max values
                slider.value = Mathf.Clamp(slider.value, slider.minValue, slider.maxValue);
            }

            
        }

    }
    public override void activate(bool down, bool up, bool held, bool first)
    {
        

        if (first)
        {
            lastPos = cursor.transform.localPosition;
        }
        

        if (held)
        {
            MoveSlider(cursor);
        }

        if(!down && up)
        {
            lastPos= Vector2.zero;
        }
    }

    public override float getCursorDistance()
    {
        if (!Settings.enable_proxemics)
        {
            if (Vector2.Distance(transform.Find("Handle Slide Area/Handle").position, cursor.transform.position) <= 0.3)
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
            return Vector3.Distance(cursor.transform.position, gameObject.transform.Find("Handle Slide Area/Handle").position);
        }


    }

    public override void setFocus(bool flag)
    {
        if (flag)
        {
            focussed = true;
            OnFocus?.Invoke(gameObject);
            if (Settings.enable_widget_outline)
            {
                transform.Find("Handle Slide Area/Handle").gameObject.GetComponent<Outline>().enabled = true;
            }
        }
        else
        {
            focussed = false;
            OnUnFocus?.Invoke(gameObject);
            transform.Find("Handle Slide Area/Handle").gameObject.GetComponent<Outline>().enabled = false; 
        }


    }

    public override void setTarget(bool flag)
    {
        if (flag)
        {
            target = true;
            SetColour(Color.green);
        }
        else
        {
            target = false;
            SetColour(Color);
        }
    }

    private float ConvertCursorDeltaToSliderValue(float cursorDelta)
    {
        // Get the screen width in Unity units
        float screenWidth = Screen.width;
        // Normalize the cursor delta based on the screen width
        float normalizedDelta = cursorDelta; /// gameObject.GetComponent<RectTransform>().rect.width;

        // Assuming the slider range is 0 to 1, no further scaling is needed
        return normalizedDelta;
    }

    float ConvertDeltaToSliderValue(float delta)
    {
        // The conversion factor could be a simple multiplication by the sensitivity
        // The sensitivity can be adjusted to control how much cursor movement is needed to move the slider from min to max
        return delta * sensitivity;
    }

    public override void SetColour(Color color)
    {
        transform.Find("Handle Slide Area/Handle").gameObject.GetComponent<Image> ().color = color;;
    }
}
