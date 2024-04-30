using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Leap;
using Leap.Unity;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class SettingsController : MonoBehaviour
{
    // Start is called before the first frame update
    private enum TrackingState {Connected, NotConnected, NoData};

    [SerializeField] TMP_Dropdown pointing;
    [SerializeField] TMP_Dropdown gesture;
    [SerializeField] TMP_Dropdown hand;
    [SerializeField] Toggle proxemics;
    [SerializeField] Toggle lightbar;
    [SerializeField] Toggle tracking;
    [SerializeField] Toggle outline;
    [SerializeField] Toggle feedback;
    [SerializeField] Toggle logging;
    [SerializeField] private System.Object calibrationScene;
    [SerializeField] Button calibrate;

    [SerializeField] TMP_InputField pinchActivation;
    [SerializeField] TMP_InputField APPosition;
    [SerializeField] TMP_InputField OcclusionOffset;
    [SerializeField] TMP_InputField DwellTime;
    [SerializeField] TMP_InputField Gain;
    [SerializeField] Slider filter;
    [SerializeField] TMP_Text filter_Value;
    [SerializeField] TMP_Dropdown cursor;
    [SerializeField] List<GameObject> Cursors = new List<GameObject>();

    [SerializeField] private GameObject statusGO;

    private InteractionType[] ps = {InteractionType.DirectMap, InteractionType.ControlDisplayGain, InteractionType.Ray, InteractionType.Debug};
    private Chirality[] cs = {Chirality.Right, Chirality.Left};
    private SelectionType[] ges = {SelectionType.Pinch, SelectionType.AirPush, SelectionType.Dwell};
    private SettingsObject set;
    private TrackingState state;
    private LeapProvider leapProvider;

    public System.Action<SettingsObject> OnSettingsUpdate;
    
    void Awake(){
        var path = Application.persistentDataPath+"/"+ SystemInfo.deviceUniqueIdentifier + ".json";
        if (File.Exists(path)){
            SettingsObject settings  = JsonUtility.FromJson<SettingsObject>(File.ReadAllText(path));
            parseSettings(settings);
        }

        leapProvider = FindObjectOfType<LeapServiceProvider>();
        
    }

    void OnEnable(){

        set = new SettingsObject();

        if(SceneManager.GetActiveScene().name.Equals("Calibration")){
            calibrate.gameObject.SetActive(false);
        }else{
             calibrate.gameObject.SetActive(true);
        }

        pinchActivation.contentType = TMP_InputField.ContentType.DecimalNumber;
        APPosition.contentType = TMP_InputField.ContentType.DecimalNumber;
        DwellTime.contentType = TMP_InputField.ContentType.IntegerNumber;
        OcclusionOffset.contentType = TMP_InputField.ContentType.IntegerNumber;
        Gain.contentType = TMP_InputField.ContentType.DecimalNumber;

        proxemics.isOn = Settings.enable_proxemics;
        lightbar.isOn = Settings.enable_light_bar;
        tracking.isOn = Settings.enable_tracking_lost;
        outline.isOn = Settings.enable_widget_outline;
        feedback.isOn = Settings.enable_widget_click_feedback;
        pointing.options.FindIndex(x => x.text.Equals(ps[Settings.pointing_method_index]));
        hand.options.FindIndex(x => x.text.Equals(cs[Settings.tracked_hand_index]));
        gesture.options.FindIndex(x => x.text.Equals(ges[Settings.gesture_index]));
        gesture.value = Settings.gesture_index;
        pointing.value = Settings.pointing_method_index;
        hand.value = Settings.tracked_hand_index;
        pointing.value = Settings.pointing_method_index;
        hand.value = Settings.tracked_hand_index;
        pinchActivation.text = Settings.pinch_distance.ToString();
        APPosition.text = Settings.airpush_position.ToString();
        DwellTime.text = Settings.dwell_time.ToString();
        OcclusionOffset.text = Settings.occlusion_offset.ToString();
        Gain.text = Settings.gain.ToString();
        filter.value = Settings.filter_strength;
        filter_Value.text = Settings.filter_strength.ToString();

        cursor.ClearOptions();
        int enumSize = System.Enum.GetValues(typeof(CursorType)).Length;
        List<TMP_Dropdown.OptionData> data = new List<TMP_Dropdown.OptionData>();
        for (int i = 0; i < enumSize; i++)
        {
            TMP_Dropdown.OptionData newData = new TMP_Dropdown.OptionData();
            newData.text = ((CursorType)i).ToString();
            data.Add(newData);
        }
        print(data);
        cursor.AddOptions(data);
        cursor.value = Settings.cursor_index;
        OnSettingsUpdate?.Invoke(set);

        var c = new Controller();
        if(c.Devices.Count == 0){
            state = TrackingState.NotConnected;
        }else if (c.Devices.Count > 0){
            state = TrackingState.Connected;
            if(leapProvider.CurrentFrame.CurrentFramesPerSecond == 0f){
                state = TrackingState.NoData;
            }
        }

        
        
        
        updateStatus(state);
    }

    void Start()
    {
        
        
        
    }

    void Update(){
        updateLayout();
    }

    public void filter_slider_change()
    {
        filter_Value.text = filter.value.ToString();
    }

    public void onCalibrateClick(){
        Settings.calibrated = false;
        onClose();
        Destroy(GameObject.Find("Service Provider"));
        Destroy(GameObject.Find("ErrorHandling"));
        Destroy(GameObject.Find("Wheel"));
        Destroy(GameObject.Find("SettingsCanvas"));
        SceneManager.LoadScene(0);
    }

    public void UpdateSettings(){
        
        Settings.enable_proxemics = proxemics.isOn;
        Settings.enable_light_bar = lightbar.isOn;
        Settings.enable_tracking_lost = tracking.isOn;
        Settings.enable_widget_outline = outline.isOn;
        Settings.enable_widget_click_feedback = feedback.isOn;
        Settings.tracked_hand_index = hand.value;
        Settings.pointing_method_index = pointing.value;
        Settings.pointing_method = ps[pointing.value];
        Settings.tracked_hand = cs[hand.value];
        Settings.gesture_index = gesture.value;
        Settings.gesture = ges[gesture.value];
        float.TryParse(pinchActivation.text, out Settings.pinch_distance);
        float.TryParse(APPosition.text, out Settings.airpush_position);
        int.TryParse(DwellTime.text, out Settings.dwell_time);
        int.TryParse(OcclusionOffset.text, out Settings.occlusion_offset);
        float.TryParse(Gain.text, out Settings.gain);
        Settings.filter_strength = filter.value;
        Settings.cursor_index = cursor.value;
        Settings.cursor = (CursorType)Settings.cursor_index;
        

    }

    public void onClose(){
        UpdateSettings();
        set.enable_proxemics = Settings.enable_proxemics;
        set.pointing_method = Settings.pointing_method;
        set.pointing_method_index = Settings.pointing_method_index;
        set.tracked_hand = Settings.tracked_hand;
        set.tracked_hand_index = Settings.tracked_hand_index;
        set.top = Settings.top;
        set.bottom = Settings.bottom;
        set.right = Settings.right;
        set.left = Settings.left;
        set.enable_light_bar = Settings.enable_light_bar;
        set.enable_tracking_lost = Settings.enable_tracking_lost;
        set.enable_widget_outline = Settings.enable_widget_outline;
        set.enable_widget_click_feedback = Settings.enable_widget_click_feedback;
        set.calibrated = Settings.calibrated;
        set.gesture = Settings.gesture;
        set.gesture_index = Settings.gesture_index;
        set.pinch_distance = Settings.pinch_distance;
        set.airpush_position = Settings.airpush_position;
        set.dwell_time = Settings.dwell_time;
        set.occlusion_offset = Settings.occlusion_offset;
        set.gain = Settings.gain;
        set.filter_strength = filter.value;
        set.cursor_type_index = cursor.value;
        set.cursor_type = Settings.cursor;
        var json = JsonUtility.ToJson(set, true);
        print(json);
        File.WriteAllText (Application.persistentDataPath+"/"+ SystemInfo.deviceUniqueIdentifier + ".json", json.ToString());
        gameObject.SetActive(false);
        OnSettingsUpdate?.Invoke(set);
        
        
    }

    public void onChange(){
        UpdateSettings();
    }

    public void parseSettings(SettingsObject fromFile){
        Settings.enable_proxemics = fromFile.enable_proxemics;
        Settings.enable_light_bar = fromFile.enable_light_bar;
        Settings.enable_tracking_lost = fromFile.enable_tracking_lost;
        Settings.enable_widget_outline = fromFile.enable_widget_outline;
        Settings.enable_widget_click_feedback = fromFile.enable_widget_click_feedback;
        Settings.tracked_hand_index = fromFile.tracked_hand_index;
        Settings.pointing_method_index = fromFile.pointing_method_index;
        Settings.pointing_method = fromFile.pointing_method;
        Settings.gesture = fromFile.gesture;
        Settings.gesture_index = fromFile.gesture_index;
        Settings.tracked_hand = fromFile.tracked_hand;
        Settings.calibrated = fromFile.calibrated;
        Settings.top = fromFile.top;
        Settings.bottom = fromFile.bottom;
        Settings.left = fromFile.left;
        Settings.right = fromFile.right;
        Settings.pinch_distance = fromFile.pinch_distance;
        Settings.airpush_position = fromFile.airpush_position;
        Settings.dwell_time =fromFile.dwell_time;
        Settings.occlusion_offset = fromFile.occlusion_offset;
        Settings.gain = fromFile.gain;
        Settings.filter_strength = fromFile.filter_strength;
        Settings.cursor_index = fromFile.cursor_type_index;
        Settings.cursor = fromFile.cursor_type;
    }

    public void updateLayout(){
        switch(Settings.gesture){
            case SelectionType.Pinch:
                pinchActivation.gameObject.transform.parent.gameObject.SetActive(true);
                APPosition.gameObject.transform.parent.gameObject.SetActive(false);
                DwellTime.gameObject.transform.parent.gameObject.SetActive(false);
                break;
            case SelectionType.AirPush:
                pinchActivation.gameObject.transform.parent.gameObject.SetActive(false);
                APPosition.gameObject.transform.parent.gameObject.SetActive(true);
                DwellTime.gameObject.transform.parent.gameObject.SetActive(false);
                break;
            case SelectionType.Dwell:
                pinchActivation.gameObject.transform.parent.gameObject.SetActive(false);
                APPosition.gameObject.transform.parent.gameObject.SetActive(false);
                DwellTime.gameObject.transform.parent.gameObject.SetActive(true);
                break;
        }

        switch(Settings.pointing_method){
            case InteractionType.DirectMap:
                OcclusionOffset.gameObject.transform.parent.gameObject.SetActive(true);
                Gain.gameObject.transform.parent.gameObject.SetActive(false);
                calibrate.gameObject.SetActive(true);
                hand.gameObject.transform.parent.gameObject.SetActive(true);
                lightbar.gameObject.transform.parent.gameObject.SetActive(true);
                break;
            case InteractionType.ControlDisplayGain:
                OcclusionOffset.gameObject.transform.parent.gameObject.SetActive(false);
                Gain.gameObject.transform.parent.gameObject.SetActive(true);
                calibrate.gameObject.SetActive(true);
                hand.gameObject.transform.parent.gameObject.SetActive(true);
                lightbar.gameObject.transform.parent.gameObject.SetActive(true);
                break;
            case InteractionType.Ray:
                throw new NotImplementedException();
            case InteractionType.Debug:
                OcclusionOffset.gameObject.transform.parent.gameObject.SetActive(false);
                Gain.gameObject.transform.parent.gameObject.SetActive(false);
                pinchActivation.gameObject.transform.parent.gameObject.SetActive(false);
                APPosition.gameObject.transform.parent.gameObject.SetActive(false);
                DwellTime.gameObject.transform.parent.gameObject.SetActive(false);
                calibrate.gameObject.SetActive(false);
                hand.gameObject.transform.parent.gameObject.SetActive(false);
                lightbar.gameObject.transform.parent.gameObject.SetActive(false);
                break;
        }

        if(Settings.pointing_method == InteractionType.Debug && Settings.gesture == SelectionType.Dwell){
            DwellTime.gameObject.transform.parent.gameObject.SetActive(true);
        }
    }

    private void updateStatus(TrackingState s){
        var indicator = statusGO.GetComponentInChildren<UnityEngine.UI.Image>();
        var text = statusGO.GetComponentInChildren<TMP_Text>();
        switch (s){
            case TrackingState.Connected:
                indicator.color = Color.green;
                text.text = "Tracking Device Connected";
                break;
            case TrackingState.NotConnected:
                indicator.color = Color.red;
                text.text = "No Tracking Device Connected";
                break;
            case TrackingState.NoData:
                indicator.color = Color.yellow;
                text.text = "Error - No Tracking Data";
                break;
        }
    }

    public void CursorTypeUpdate()
    {
        Settings.cursor_index = cursor.value;
        var list_cursor = GameObject.FindGameObjectWithTag("Cursor");
        list_cursor.GetComponent<CursorManager>().updateCursor((CursorType)Settings.cursor_index);
    }


}

public class SettingsObject{

    public bool enable_proxemics;
    public InteractionType pointing_method;
    public int pointing_method_index;
    public Chirality tracked_hand;
    public int tracked_hand_index;
    public SelectionType gesture;
    public int gesture_index;
    public bool calibrated;
    public Vector2 top, bottom, left, right;
    public bool enable_light_bar;
    public bool enable_tracking_lost;
    public bool enable_widget_outline;
    public bool enable_widget_click_feedback;
    public bool enable_logging;
    public float gain;
    public float pinch_distance;
    public float airpush_position;
    public int dwell_time;
    public int occlusion_offset;
    public float filter_strength;
    public int cursor_type_index;
    public CursorType cursor_type;
}


