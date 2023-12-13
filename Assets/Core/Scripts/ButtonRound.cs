using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonRound : ButtonController
{
    // Start is called before the first frame update
    void OnEnable(){
        type = Type.Button;
        shape = Shape.Circle;
        
    }
}
