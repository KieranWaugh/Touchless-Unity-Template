using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public List<GameObject> cursors = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        updateCursor(Settings.cursor);
    }

    // Update is called once per frame
    public void updateCursor(CursorType type)
    {
        print(type.ToString());
        foreach (GameObject cursor in cursors)
        {
            cursor.SetActive(false);
            if(cursor.GetComponent<Cursor>().cursorType == type)
            {
                cursor.SetActive(true);
            }
        }
    }
}
