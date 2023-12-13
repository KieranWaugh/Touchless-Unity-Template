using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSquare : ButtonController
{
    // Start is called before the first frame update
    void OnEnable(){
        type = Type.Button;
        shape = Shape.Rectangle;
    }
}
