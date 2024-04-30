using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CursorManager : MonoBehaviour
{
    public List<GameObject> cursors = new List<GameObject>();
    [SerializeField] private InteractionManager interactionManager;
    

    // Start is called before the first frame update
    void Start()
    {
        interactionManager = GameObject.Find("Service Provider").GetComponent<InteractionManager>();
        updateCursor(Settings.cursor);
        
        
    }

    // Update is called once per frame
    public void updateCursor(CursorType type)
    {
        print(type.ToString());
        foreach (GameObject cursor in cursors)
        {
            cursor.SetActive(false);
            if((CursorType)cursor.GetComponent<Cursor>().cursorType == type)
            {
                cursor.SetActive(true);
                interactionManager.setCursor(cursor.GetComponent<Cursor>().InteractionPoint);
                print(cursor.GetComponent<Cursor>().InteractionPoint);
            }
        }
    }
}
public enum CursorType { Standard, Radial }
