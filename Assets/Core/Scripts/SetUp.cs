using System.Collections;
using System.Collections.Generic;
using System.IO;
using Leap;
using Leap.Unity;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

public class SetUp : MonoBehaviour
{

    [SerializeField] private GameObject T1, T2, T3, T4;
    [SerializeField] private InteractionManager interactionManager;
    [SerializeField] private Object UI;
    [SerializeField] private GameObject progressBar;
    private GameObject settings;
    
    private progressBar progressBarController;
    private int targetIndex = 0;
    private Vector2 T1PH, T2PH, T3PH, T4PH;
    private Vector2 T1P, T2P, T3P, T4P;
    private Vector2 currentHandPos;
    private Hand hand;
    private int handCount;
    private WheelController button;

    // Start is called before the first frame update

    void OnEnable(){
        interactionManager.PositionUpdate += positionUpdate;
        button = GameObject.Find("Wheel").GetComponent<WheelController>();
        button.onButtonPress += targetClicked;
    }

    void OnDisable(){
       interactionManager.PositionUpdate -= positionUpdate;
       button.onButtonPress -= targetClicked;
    }

    void Start()
    {

        if(Settings.calibrated){
            targetIndex = 4;
            print("calibrated");
        }
        

        T1P = T1.transform.position;
        T2P = T2.transform.position;
        T3P = T3.transform.position;
        T4P = T4.transform.position;
        progressBarController = progressBar.GetComponent<progressBar>();
        progressBarController.maxValue = 4;
        progressBarController.currentValue = 1;

        settings = GameObject.FindGameObjectWithTag("Settings");



    }

    // Update is called once per frame
    void Update()
    {
        
        

        if(Settings.pointing_method == InteractionType.Debug){
            targetIndex = 4;
        }
        if(targetIndex == 4){
            gameObject.SetActive(false);
            progressBarController.updateBar(targetIndex + 1);
            Settings.calibrated = true;
            interactionManager.settings.GetComponent<SettingsController>().onClose();

            Settings.ran_calibration_scene = true;
            SceneManager.LoadScene(1);
        }

        switch (targetIndex){
            case 0:
                T1.SetActive(true);
                T2.SetActive(false);
                T3.SetActive(false);
                T4.SetActive(false);
                break;
            case 1:
                T1.SetActive(false);
                T2.SetActive(true);
                T3.SetActive(false);
                T4.SetActive(false);
                break;
            case 2:
                T1.SetActive(false);
                T2.SetActive(false);
                T3.SetActive(true);
                T4.SetActive(false);
                break;
            case 3:
                T1.SetActive(false);
                T2.SetActive(false);
                T3.SetActive(false);
                T4.SetActive(true);
                break;
        }
        if(Input.GetKeyDown(KeyCode.Space) && hand.GetChirality().Equals(Settings.tracked_hand)){
            targetClicked();
            
        }

        

        
    }

    void positionUpdate(Hand pos, int hands){
        
        hand = pos;
        handCount = hands;
        if(handCount != 0){
        currentHandPos = pos.GetIndex().TipPosition;
        }
        
        

        
    }

    public void targetClicked(){
        if(handCount == 0){

            }else{
            switch (targetIndex){
                case 0:
                    T1PH = currentHandPos;
                    print("Frame - " + T1PH);
                    print("Hand - " + interactionManager.right_indexTip.transform.position);
                    Settings.top = T1PH;
                    targetIndex++;
                    break;
                case 1:
                    T2PH = currentHandPos;
                    Settings.right = T2PH;
                    print("Frame - " + T2PH);
                    print("Hand - " + interactionManager.right_indexTip.transform.position);
                    targetIndex++;
                    break;
                case 2:
                    T3PH = currentHandPos;
                    Settings.bottom = T3PH;
                    print("Frame - " + T3PH);
                    print("Hand - " + interactionManager.right_indexTip.transform.position);
                    targetIndex++;
                    break;
                case 3:
                    T4PH = currentHandPos;
                    Settings.left = T4PH;
                    print("Frame - " + T4PH);
                    print("Hand - " + interactionManager.right_indexTip.transform.position);
                    targetIndex++;

                    break;
            }
            }
    }

    
}
