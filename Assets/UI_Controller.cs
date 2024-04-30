using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Controller : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        if (!Settings.ran_calibration_scene)
        {
            SceneManager.LoadScene(0);
        }
    }

    
}
