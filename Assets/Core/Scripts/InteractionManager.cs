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
public enum TrackedPosition { PinchPoint, Index, Palm };
public class InteractionManager : MonoBehaviour
{
    

#region Fields
    [SerializeField] public LeapProvider leapProvider;
    [SerializeField] private GameObject trackingLost;
    [SerializeField] private GameObject dataLost;
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
    private bool calledFirstUpdate = false;
    private screenHand screen_positions = new screenHand();

    public GameObject focusedWidget;
    private bool widgetActive = false;


    [SerializeField] public List<GameObject> widgets = new List<GameObject>();
    

    public Action<Hand, int> PositionUpdate;
    public Action<screenHand, Frame> CursorUpdate;
    private GameObject canvas;
    public GameObject settings;
    Vector3[] worldCorners = new Vector3[4];
    private float maxX, maxY, minX, minY; 
    public bool settingsActive = false;
    private GestureDetector gestureDetector;
    public GameObject cursor; // used to identify the cursor part of the cursors

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

    private KalmanFilter pinch_kalman;
    private KalmanFilter index_kalman;
    private KalmanFilter thumb_kalman;
    private KalmanFilter palm_kalman;


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

        pinch_kalman = new KalmanFilter(Mathf.Pow(10, -Settings.filter_strength), 0.001f);
        index_kalman = new KalmanFilter(Mathf.Pow(10, -Settings.filter_strength), 0.001f);
        thumb_kalman = new KalmanFilter(Mathf.Pow(10, -Settings.filter_strength), 0.001f);
        palm_kalman = new KalmanFilter(Mathf.Pow(10, -Settings.filter_strength), 0.001f);

    }

  

    void Update(){
        if (!calledFirstUpdate)
        {
            calledFirstUpdate = true;
        }

        if(Settings.pointing_method == InteractionType.Debug){
            Settings.cursor = CursorType.Standard;
            Settings.enable_tracking_lost = false;
            var mp = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
            Vector3 objectPos = new Vector2
            {
                x = Mathf.Clamp(mp.x, minX, maxX),
                y = Mathf.Clamp(mp.y, minY, maxY)
            };
            if (!settingsActive){
                screen_positions.PinchPoint = objectPos;
                CursorUpdate?.Invoke(screen_positions, null);
            }
            
            trackingLost.SetActive(false);
            dataLost.SetActive(false);


        }
        else
        

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
            dataLost.SetActive(false);
            dataLost.SetActive(false);
            foreach (GameObject w in widgets){
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


        if(frame.CurrentFramesPerSecond == 0f)
        {
            trackingLost.SetActive(false);
            if(Settings.pointing_method != InteractionType.Debug)
            {
                dataLost.SetActive(true);
            }
            
        }
        else
        {
            dataLost.SetActive(false);
        }

        if (frame.Hands.Count != 0 && !settingsActive ){
            dataLost.SetActive(false);
            
            if((frame.Hands.Count == 1 && frame.Hands[0].GetChirality() == Settings.tracked_hand) || frame.Hands.Count == 2){

                

                LightBarController.SetActive(Settings.enable_light_bar);
                    if(LightBarController != null){
                    LightBarController.GetComponent<LocationController>().updateColour(frame.Hands[0].PalmPosition);
                    }
                
                PositionUpdate?.Invoke(frame.Hands[0], frame.Hands.Count);
                trackingLost.SetActive(false);
                click = false;

                if(Settings.calibrated){
                    getScreenPositions(frame);
                    CursorUpdate?.Invoke(screen_positions, frame);

                }

            }
            else{
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

    public void setCursor(GameObject trackedPos)
    {
        cursor = trackedPos;
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

    public screenHand getScreenPositions(Frame frame)
    {
        float gain = 1f;

        if(Settings.pointing_method == InteractionType.ControlDisplayGain)
        {
            gain = Settings.gain;
        }

        var PinchPoint = new Vector3(map(frame.GetHand(Settings.tracked_hand).GetPredictedPinchPosition().x, Settings.left.x, Settings.right.x, minX, maxX), map(frame.GetHand(Settings.tracked_hand).GetPredictedPinchPosition().y, Settings.bottom.y, Settings.top.y, minY, maxY));
        var Index = new Vector3(map(frame.GetHand(Settings.tracked_hand).GetIndex().TipPosition.x, Settings.left.x, Settings.right.x, minX, maxX), map(frame.GetHand(Settings.tracked_hand).GetIndex().TipPosition.y, Settings.bottom.y, Settings.top.y, minY, maxY));
        var Thumb = new Vector3(map(frame.GetHand(Settings.tracked_hand).GetThumb().TipPosition.x, Settings.left.x, Settings.right.x, minX, maxX), map(frame.GetHand(Settings.tracked_hand).GetThumb().TipPosition.y, Settings.bottom.y, Settings.top.y, minY, maxY));
        var Palm = new Vector3(map(frame.GetHand(Settings.tracked_hand).PalmPosition.x, Settings.left.x, Settings.right.x, minX, maxX), map(frame.GetHand(Settings.tracked_hand).PalmPosition.y, Settings.bottom.y, Settings.top.y, minY, maxY));
        var offset = Settings.occlusion_offset;

        if(Settings.tracked_hand == Chirality.Right)
        {
            offset = -1 * offset;
        }


        screen_positions.PinchPoint = pinch_kalman.UpdateFilter(new Vector3(Mathf.Clamp(PinchPoint.x + Settings.occlusion_offset, minX, maxX), Mathf.Clamp(PinchPoint.y, minY, maxY), 0), Mathf.Pow(10, -Settings.filter_strength), 0.001f) * gain;
        screen_positions.Index = index_kalman.UpdateFilter(new Vector3(Mathf.Clamp(Index.x + Settings.occlusion_offset, minX, maxX), Mathf.Clamp(Index.y, minY, maxY), 0), Mathf.Pow(10, -Settings.filter_strength), 0.001f) * gain;
        screen_positions.Thumb = thumb_kalman.UpdateFilter(new Vector3(Mathf.Clamp(Thumb.x + Settings.occlusion_offset, minX, maxX), Mathf.Clamp(Thumb.y, minY, maxY), 0), Mathf.Pow(10, -Settings.filter_strength), 0.001f) * gain;
        screen_positions.Palm = palm_kalman.UpdateFilter(new Vector3(Mathf.Clamp(Palm.x + Settings.occlusion_offset, minX, maxX), Mathf.Clamp(Palm.y, minY, maxY), 0), Mathf.Pow(10, -Settings.filter_strength), 0.001f) * gain;

        //screen_positions.PinchPoint = PinchPoint;
        //screen_positions.Index = Index;
        //screen_positions.Thumb = Thumb;
        //screen_positions.Palm = Palm;

        return screen_positions;

    }

    




}

public class screenHand {

    
    public Vector3 PinchPoint { get; set; }
    public Vector3 Index { get; set; }
    public Vector3 Thumb { get; set; }
    public Vector3 Palm { get; set; }

    public screenHand()
    {
        
    }

    public Vector3 getTrackedPosition(TrackedPosition position)
    {
        switch (position)
        {
            case TrackedPosition.PinchPoint:
                return PinchPoint;
            case TrackedPosition.Index:
                return Index;
            case TrackedPosition.Palm:
                return Palm;
            default:
                return PinchPoint;
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
