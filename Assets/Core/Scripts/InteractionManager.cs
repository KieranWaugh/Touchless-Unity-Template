using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;
using System;
using UnityEngine.SceneManagement;
using System.Linq;
using Leap.Unity.Attachments;

public enum InteractionType { DirectMap, Ray, ControlDisplayGain, Debug };
public enum SelectionType{Pinch, AirPush, Dwell}
public class InteractionManager : MonoBehaviour
{
    

#region Fields
    [SerializeField] public LeapProvider leapProvider;
    [SerializeField] private GameObject trackingLost;
    [SerializeField] private GameObject LightBarController;
    [SerializeField] public InteractionType pointingMethod;
    [SerializeField] public PinchDetector pinchDetector;
    [SerializeField] public AirPushDetector airPushDetector;
    [SerializeField] public DebugDetector debugDetector;
    [SerializeField] private int occlusionOffset = Settings.occlusion_offset;
    public bool gesture = false;
    public bool click = false;
    public  bool held;
    private Controller c;

    public GameObject focusedWidget;
    private bool widgetActive = false;


    [SerializeField] public List<GameObject> widgets = new List<GameObject>();
    

    public Action<Hand, int> PositionUpdate;
    public Action<Vector3> CursorUpdate;
    private GameObject canvas;
    public GameObject settings;
    Vector3[] worldCorners = new Vector3[4];
    private float maxX, maxY, minX, minY; 
    public bool settingsActive = false;
    private GestureDetector gestureDetector;

    // Hand Locations
    [SerializeField] private AttachmentHands hand;
    [SerializeField] public GameObject left_indexTip;
    [SerializeField] public GameObject left_thumbTip;
    [SerializeField] public GameObject left_palm;
    [SerializeField] public GameObject left_pinchPoint;

    [SerializeField] public GameObject right_indexTip;
    [SerializeField] public GameObject right_thumbTip;
    [SerializeField] public GameObject right_palm;
    [SerializeField] public GameObject right_pinchPoint;

    // Start is called before the first frame update
    #endregion

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

   private void OnEnable()
    {



        leapProvider.OnUpdateFrame += OnUpdateFrame;
       

        

        
        SceneManager.activeSceneChanged += ChangedActiveScene;
        canvas = GameObject.FindGameObjectWithTag("Canvas");
        settings = GameObject.FindGameObjectWithTag("Settings");
        settings.GetComponent<SettingsController>().OnSettingsUpdate += OnSettingsUpdate;
        updateInteractions();
        c = new Controller();
        
    }

    void OnDisable(){
        leapProvider.OnUpdateFrame -= OnUpdateFrame;
        gestureDetector.OnGesture -= OnGesture;
        gestureDetector.OnUnGesture -= OnUnGesture;
        gestureDetector.OnHeld -= gestureHeld;
        SceneManager.activeSceneChanged -= ChangedActiveScene;
        leapProvider.OnUpdateFrame -= OnUpdateFrame;
    }


    void Start(){
        canvas = GameObject.FindGameObjectWithTag("Canvas");
        settings = GameObject.FindGameObjectWithTag("Settings");
        settings.SetActive(false);
        canvas.GetComponent<RectTransform>().GetLocalCorners(worldCorners);
        maxX = worldCorners[3].x;
        maxY = worldCorners[1].y;
        minX = worldCorners[1].x;
        minY = worldCorners[3].y;
        LightBarController.SetActive(Settings.enable_light_bar);
        updateInteractions();
        
    }

    void Update(){
        if(Settings.pointing_method == InteractionType.Debug){
            var mp = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
            Vector3 objectPos = new Vector2
            {
                x = Mathf.Clamp(mp.x, minX, maxX),
                y = Mathf.Clamp(mp.y, minY, maxY)
            };
            if (!settingsActive){
                CursorUpdate?.Invoke(objectPos);
            }
            
            trackingLost.SetActive(false);
            
            
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingsActive)
            {
                settings.GetComponent<SettingsController>().onClose();
                settings.SetActive(false);

            }
            else
            {
                Application.Quit();

            }
        }

        if(Input.GetKeyDown(KeyCode.S)){
            settingsActive = true;
            settings.SetActive(true);
            trackingLost.SetActive(false);
            foreach(GameObject w in widgets){
                w.GetComponent<Widget>().SetWidgetActive(false);
            }
        }

        if (widgets.Count != 0 && !widgetActive)
        {
            widgets = widgets.OrderBy(go => go.GetComponent<Widget>().getDistance()).ToList();

            if(widgets[0].GetComponent<Widget>().getDistance() != -1)
            {
                focusedWidget = widgets[0];
                focusedWidget.GetComponent<Widget>().setFocus(true);

            }
            else
            {
                widgets[0].GetComponent<Widget>().setFocus(false);
                focusedWidget = null;
            }
            
            for (int i = 1; i < widgets.Count; i++)
            {
                widgets[i].GetComponent<Widget>().setFocus(false);
            }

        }
        else
        {
            if (!widgetActive)
            {
                focusedWidget = null;
            }
            
        }
        

    }

    void OnUpdateFrame(Frame frame)
    {
        if(frame.Hands.Count != 0 && !settingsActive ){

            if((frame.Hands.Count == 1 && frame.Hands[0].GetChirality() == Settings.tracked_hand) || frame.Hands.Count == 2){

                

                LightBarController.SetActive(Settings.enable_light_bar);
                    if(LightBarController != null){
                    LightBarController.GetComponent<LocationController>().updateColour(frame.Hands[0].PalmPosition);
                    }
                
                PositionUpdate?.Invoke(frame.Hands[0], frame.Hands.Count);
                trackingLost.SetActive(false);
                click = false;

                if(Settings.calibrated){
                    float cursorX = map(frame.GetHand(Settings.tracked_hand).GetPinchPosition().x, Settings.left.x, Settings.right.x, minX, maxX);
                    float cursorY = map(frame.GetHand(Settings.tracked_hand).GetPinchPosition().y, Settings.bottom.y, Settings.top.y, minY, maxY);
                    
                    
                    Vector3 objectPos = new Vector2();
                    if(!settingsActive){
                        switch(Settings.tracked_hand){ 
                        case Chirality.Right:
                            objectPos = new Vector2
                            {
                                // Constrain the object's position to the boundaries of the canvas
                                x = Mathf.Clamp(cursorX - Settings.occlusion_offset, minX, maxX),
                                y = Mathf.Clamp(cursorY, minY, maxY)
                            };
                            CursorUpdate?.Invoke(new Vector3(objectPos.x, objectPos.y,0));
                            break;
                        case Chirality.Left:
                            objectPos = new Vector2
                            {
                                // Constrain the object's position to the boundaries of the canvas
                                x = Mathf.Clamp(cursorX + Settings.occlusion_offset, minX, maxX),
                                y = Mathf.Clamp(cursorY, minY, maxY)
                            };
                            CursorUpdate?.Invoke(new Vector3(objectPos.x, objectPos.y,0));
                            break;
                    }
                    }
                    
                    
                }

            }else{
                if(Settings.enable_tracking_lost){
                    if(!settingsActive){
                        trackingLost.SetActive(true);
                        PositionUpdate?.Invoke(null, 0);
                    }
                
            }
            }

        }else{
            if(Settings.enable_tracking_lost){
                if(!settingsActive){
                    trackingLost.SetActive(true);
                    PositionUpdate?.Invoke(null, 0);
                }
                
            }
            
        }
        
    }

    static float map(float value, float istart, float istop, float ostart, float ostop)
    {
        return ostart + (ostop - ostart) * ((value - istart) / (istop - istart));
    }

    public void addWidget(GameObject widget){
        widgets.Add(widget);
    }

    void OnGesture(Hand hand){
        gesture = true;
        if(focusedWidget != null && !widgetActive)
        {
            focusedWidget.GetComponent<Widget>().activate(gesture, click, held, true);
            widgetActive = true;
        }
        
        

    }

    void OnUnGesture(Hand hand){
        gesture = false;
        click = true;
        held = false;
        if (focusedWidget != null && widgetActive)
        {
            focusedWidget.GetComponent<Widget>().activate(gesture, click, held, false);
            widgetActive = false;
        }
        click = false;
        

    }

    void gestureHeld(Hand hand){
        held = true;
        if(focusedWidget != null)
            {
                focusedWidget.GetComponent<Widget>().activate(gesture, click, held, false);
            }
    }

    private void ChangedActiveScene(Scene arg0, Scene arg1)
    {
        canvas = GameObject.FindGameObjectWithTag("Canvas");
    }

    // public void settingsClosed(){
    //     settingsActive = false;
    //     foreach(GameObject w in widgets){
    //             w.GetComponent<Widget>().SetWidgetActive(true);
    //     }
 
    // }

    void OnSettingsUpdate(SettingsObject s){
        //parseSettings(s);
        updateInteractions();
    }

    void updateInteractions(){
        switch (Settings.gesture){
            case SelectionType.Pinch:
                gestureDetector = pinchDetector;
                pinchDetector.enabled = true;
                airPushDetector.enabled = false;

                if(Settings.pointing_method == InteractionType.Debug){
                    gestureDetector = debugDetector;
                    pinchDetector.enabled = false;
                    airPushDetector.enabled = false;
                    debugDetector.enabled = true;
                }

                break;
            case SelectionType.AirPush:
                gestureDetector = airPushDetector;
                pinchDetector.enabled = false;
                airPushDetector.enabled = true;

                if(Settings.pointing_method == InteractionType.Debug){
                    gestureDetector = debugDetector;
                    pinchDetector.enabled = false;
                    airPushDetector.enabled = false;
                    debugDetector.enabled = true;
                }

                break;
        }

        gestureDetector.OnGesture += OnGesture;
        gestureDetector.OnUnGesture += OnUnGesture;
        gestureDetector.OnHeld += gestureHeld;

        if(!settings.activeInHierarchy){
            settingsActive = false;
            foreach(GameObject w in widgets){
                    w.GetComponent<Widget>().SetWidgetActive(true);
            }
        }


    }

    


}

//class HandsModule
//{
//    private AttachmentHand left;
//    private AttachmentHand right;
//    private Hand trackedHand;

//    public HandsModule(AttachmentHand left, AttachmentHand right, Hand trackedHand)
//    {
//        this.left = left;
//        this.right = right;
//        this.trackedHand = trackedHand;
//    }

//    public Vector3 getPosition(Chirality chirality, AttachmentPointFlags point)
//    {
//        if(chirality == Chirality.Left)
//        {
//            Vector3 pos;
//            Quaternion rot;
//            left.GetComponent<AttachmentHand>().GetBehaviourForPoint(point).GetLeapHandPointData(trackedHand, point, out pos, out rot);
//        }
//    }
//}
