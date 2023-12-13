using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.SceneManagement;

public class test : MonoBehaviour
{
    [SerializeField] GameObject canvas;
    private int counter = 0;
    // Start is called before the first frame update

    void OnEnable(){
        var t =  FindObjectsOfType<ButtonController>();
        foreach (ButtonController g in t)
        {
            g.buttonPress += onClick;
            
        }
    }
    void Start()
    {

        

        

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void blah (){
        
        print("clciked ");
    }

    public void ghgh(GameObject g){
        print("sent " + g.name);
    }

    void onClick(GameObject b){
        print("Pressed " + b.name + " " + counter);
        counter++;
    }
}


