using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TouchlessInputController : MonoBehaviour
{
    // Start is called before the first frame update
    public Action<Vector3, bool> PositionUpdate;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Poll();
    }

    public abstract void Poll(); //poll device for hand movement changes
}
