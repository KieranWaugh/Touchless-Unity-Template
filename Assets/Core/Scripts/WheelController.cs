using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelController : MonoBehaviour
{
    // Start is called before the first frame update
    public Action onButtonPress;

    void Awake(){
        DontDestroyOnLoad(gameObject);
    }

    public void onPress(){
        onButtonPress?.Invoke();
    }
}
