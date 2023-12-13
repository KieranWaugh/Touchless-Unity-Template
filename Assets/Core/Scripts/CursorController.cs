using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using UnityEngine.SceneManagement;

public class CursorController : MonoBehaviour
{
    [SerializeField] private InteractionManager interactionManager;
    [SerializeField] private Camera cam;
    public Vector2[] pos;
    

    // Start is called before the first frame update
    void OnEnable(){
        interactionManager = GameObject.Find("Service Provider").GetComponent<InteractionManager>();
        interactionManager.CursorUpdate += CursorUpdate;


    }

    void Start(){
        if(interactionManager == null){
                SceneManager.LoadScene(0);
        }
        pos = new Vector2[2];
        pos[0] = transform.position;
        pos[1] = transform.position;

    }

    void OnDisable(){
        
    }

    void Update(){
        if(interactionManager.gesture && !interactionManager.settings.activeInHierarchy){
            gameObject.GetComponentInChildren<RectTransform>().sizeDelta = new Vector2(20,20);
        }else{
            gameObject.GetComponentInChildren<RectTransform>().sizeDelta = new Vector2(30,30);
        }

         
    }

    void CursorUpdate(Vector3 position){
        
        if (Settings.pointing_method == InteractionType.Debug){
            if(interactionManager.settings.activeInHierarchy){
                gameObject.transform.localPosition = position;
                pos[1] = pos[0];
                pos[0] = position;
            }else{
                gameObject.transform.position = position;
                pos[1] = pos[0];
                pos[0] = position;
            }
            
        }else if(Settings.pointing_method == InteractionType.ControlDisplayGain){
            position = Settings.gain * position;
            gameObject.transform.localPosition = position;
            pos[1] = pos[0];
            pos[0] = position;
        }
        
        else{
            gameObject.transform.localPosition = position;
            pos[1] = pos[0];
            pos[0] = position;
        }
        
    }

    
}


